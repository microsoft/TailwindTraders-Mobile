using Emgu.TF.Lite;
using PubSub.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TailwindTraders.Mobile.Features.Logging;
using TailwindTraders.Mobile.Features.Scanning;
using TailwindTraders.Mobile.Features.Scanning.AR;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using Xamarin.Forms;

[assembly: Dependency(typeof(TensorflowLiteService))]

namespace TailwindTraders.Mobile.Features.Scanning
{
    public class TensorflowLiteService
    {
        // private const string ImageFilename = "AR/images/IMG_20181220_104230.jpg";
        private const string LabelFilename = "AR/pets/labels_list.txt";
        private const string ModelFilename = "AR/pets/detect.tflite";

        public const int ModelInputSize = 300;
        private const float MinScore = 0.4f;
        private const bool QuantizedModel = true;
        private const int LabelOffset = 1;

        private bool initialized = false;
        private string[] labels = null;
        private FlatBufferModel model;
        private bool useNumThreads;

        private IPlatformService platformService;
        private ILoggingService loggingService;

        public TensorflowLiteService()
        {
            platformService = DependencyService.Get<IPlatformService>();
            loggingService = DependencyService.Get<ILoggingService>();

            Initialize();
        }

        public static void DoNotStripMe()
        {
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            useNumThreads = Device.RuntimePlatform == Device.Android;

            var labelPath = platformService.CopyToFilesAndGetPath(LabelFilename);
            var labelData = File.ReadAllBytes(labelPath);
            var labelContent = Encoding.Default.GetString(labelData);

            labels = labelContent
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

            var modelFileName = platformService.CopyToFilesAndGetPath(ModelFilename);
            model = new FlatBufferModel(modelFileName);
            if (!model.CheckModelIdentifier())
            {
                throw new Exception("Model identifier check failed");
            }

            initialized = true;
        }

        /*public void RecognizeBuiltInImage()
        {
            var imagePath = platformService.CopyToFilesAndGetPath(ImageFilename);
            var imageData = File.ReadAllBytes(imagePath);

            Recognize(imageData);
        }*/

        public void Recognize(byte[] imageData, int rotation)
        {
            using (var op = new BuildinOpResolver())
            {
                using (var interpreter = new Interpreter(model, op))
                {
                    InvokeInterpreter(imageData, interpreter, rotation);
                }
            }
        }

        private void InvokeInterpreter(byte[] imageData, Interpreter interpreter, int rotation)
        {
            if (useNumThreads)
            {
                interpreter.SetNumThreads(Environment.ProcessorCount);
            }

            var allocateTensorStatus = interpreter.AllocateTensors();
            if (allocateTensorStatus == Status.Error)
            {
                throw new Exception("Failed to allocate tensor");
            }

            var input = interpreter.GetInput();
            using (var inputTensor = interpreter.GetTensor(input[0]))
            {
                // TODO: Optimize this!
                var watchReadImageFileToTensor = Stopwatch.StartNew();
                platformService.ReadImageFileToTensor(
                    imageData,
                    QuantizedModel,
                    inputTensor.DataPointer,
                    ModelInputSize,
                    ModelInputSize,
                    rotation);
                watchReadImageFileToTensor.Stop();

                loggingService.Debug($"ReadImageFileToTensor: {watchReadImageFileToTensor.ElapsedMilliseconds}ms");

                var watchInvoke = Stopwatch.StartNew();
                interpreter.Invoke();
                watchInvoke.Stop();

                loggingService.Debug($"InterpreterInvoke: {watchInvoke.ElapsedMilliseconds}ms");
            }

            var output = interpreter.GetOutput();
            var outputIndex = output[0];

            var outputTensors = new Tensor[output.Length];
            for (var i = 0; i < output.Length; i++)
            {
                outputTensors[i] = interpreter.GetTensor(outputIndex + i);
            }

            var detection_boxes_out = outputTensors[0].GetData() as float[];
            var detection_classes_out = outputTensors[1].GetData() as float[];
            var detection_scores_out = outputTensors[2].GetData() as float[];
            var num_detections_out = outputTensors[3].GetData() as float[];

            var numDetections = num_detections_out[0];

            LogDetectionResults(detection_classes_out, detection_scores_out, detection_boxes_out, numDetections);

            for (var i = 0; i < output.Length; i++)
            {
                outputTensors[i].Dispose();
            }
        }

        private void LogDetectionResults(
            float[] detection_classes_out,
            float[] detection_scores_out,
            float[] detection_boxes_out,
            float numDetections)
        {
            ////loggingService.Debug($"NumDetections: {numDetections}");

            for (int i = 0; i < numDetections; i++)
            {
                var score = detection_scores_out[i];
                var classId = (int)detection_classes_out[i];

                ////loggingService.Debug($"Found classId({classId}) with score({score})");

                if (classId >= 0 && classId < labels.Length)
                {
                    var label = labels[classId + LabelOffset];
                    if (score >= MinScore)
                    {
                        ////var top = box[0] * height;
                        ////var left = box[1] * width;
                        ////var bottom = box[2] * height;
                        ////var right = box[3] * width;

                        var xmin = detection_boxes_out[0];
                        var ymin = detection_boxes_out[1];
                        var xmax = detection_boxes_out[2];
                        var ymax = detection_boxes_out[3];

                        this.Publish(new BoundingBoxMessageArgs()
                        {
                            Xmin = xmin,
                            Ymin = ymin,
                            Xmax = xmax,
                            Ymax = ymax,
                        });

                        loggingService.Debug($"{label} with score {score} " +
                            $"with detection boxes: {xmin} {ymin} {xmax} {ymax}");
                    }
                }
            }
        }
    }
}
