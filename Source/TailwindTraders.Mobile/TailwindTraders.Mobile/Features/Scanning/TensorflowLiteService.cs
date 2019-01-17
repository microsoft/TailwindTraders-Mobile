using Emgu.TF.Lite;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TailwindTraders.Mobile.Features.Scanning;
using TailwindTraders.Mobile.Features.Scanning.AR;
using Xamarin.Forms;

[assembly: Dependency(typeof(TensorflowLiteService))]
namespace TailwindTraders.Mobile.Features.Scanning
{
    public class TensorflowLiteService
    {
        public const string TFFolder = "AR/pets/";
        public const int ModelInputSize = 300;
        public const float MinScore = 0.6f;
        public const string ObjectDetectedMessage = nameof(ObjectDetectedMessage);

        public readonly string LabelFilename = TFFolder + "labels_list.txt";
        public readonly string ModelFilename = TFFolder + "detect.tflite";

        private const int LabelOffset = 1;

        private byte[] quantizedColors;
        private bool initialized = false;
        private string[] labels = null;
        private FlatBufferModel model;
        private bool useNumThreads;

        public static void DoNotStripMe()
        {
        }

        public void Initialize(string labelPath, string modelPath)
        {
            if (initialized)
            {
                return;
            }

            useNumThreads = Device.RuntimePlatform == Device.Android;

            var labelData = File.ReadAllBytes(labelPath);
            var labelContent = Encoding.Default.GetString(labelData);

            labels = labelContent
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

            model = new FlatBufferModel(modelPath);
            if (!model.CheckModelIdentifier())
            {
                throw new Exception("Model identifier check failed");
            }

            quantizedColors = new byte[ModelInputSize * ModelInputSize * 3];

            initialized = true;
        }

        public void Recognize(int[] colors)
        {
            if (!initialized)
            {
                throw new Exception("Initialize TensorflowLiteService first");
            }

            using (var op = new BuildinOpResolver())
            {
                using (var interpreter = new Interpreter(model, op))
                {
                    InvokeInterpreter(colors, interpreter);
                }
            }
        }

        private void InvokeInterpreter(int[] colors, Interpreter interpreter)
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
                CopyColorsToTensor(inputTensor.DataPointer, colors);

                var watchInvoke = Stopwatch.StartNew();
                interpreter.Invoke();
                watchInvoke.Stop();

                Console.WriteLine($"InterpreterInvoke: {watchInvoke.ElapsedMilliseconds}ms");
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
        }

        private void CopyColorsToTensor(IntPtr dest, int[] colors)
        {
            for (var i = 0; i < colors.Length; ++i)
            {
                var val = colors[i];

                //// AA RR GG BB
                var r = (byte)((val >> 16) & 0xFF);
                var g = (byte)((val >> 8) & 0xFF);
                var b = (byte)(val & 0xFF);

                quantizedColors[(i * 3) + 0] = r;
                quantizedColors[(i * 3) + 1] = g;
                quantizedColors[(i * 3) + 2] = b;
            }

            System.Runtime.InteropServices.Marshal.Copy(quantizedColors, 0, dest, quantizedColors.Length);
        }

        private void LogDetectionResults(
            float[] detection_classes_out,
            float[] detection_scores_out,
            float[] detection_boxes_out,
            float numDetections)
        {
            for (int i = 0; i < numDetections; i++)
            {
                var score = detection_scores_out[i];
                var classId = (int)detection_classes_out[i];

                //// Console.WriteLine($"Found classId({classId}) with score({score})");

                if (classId >= 0 && classId < labels.Length)
                {
                    var label = labels[classId + LabelOffset];
                    if (score >= MinScore)
                    {
                        var xmin = detection_boxes_out[0];
                        var ymin = detection_boxes_out[1];
                        var xmax = detection_boxes_out[2];
                        var ymax = detection_boxes_out[3];

                        MessagingCenter.Instance.Send(this, nameof(ObjectDetectedMessage), new DetectionMessage
                        {
                            Xmin = xmin,
                            Ymin = ymin,
                            Xmax = xmax,
                            Ymax = ymax,
                            Score = score,
                            Label = label,
                        });

                        Console.WriteLine($"{label} with score {score} " +
                            $"with detection boxes: {xmin} {ymin} {xmax} {ymax}");
                    }
                }
            }
        }
    }
}
