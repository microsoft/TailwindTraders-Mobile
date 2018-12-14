//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Emgu.TF.Lite
{
    public static partial class TfLiteInvoke
    {
        [UnmanagedFunctionPointer(Constants.TFCallingConvention)]
        public delegate int TfliteErrorCallback(int status, IntPtr errMsg);

        [MonoPInvokeCallback(typeof(TfliteErrorCallback))]
        private static int TfliteErrorHandler(
            int status,
            IntPtr errMsg)
        {
            // TODO
            // String msg = Marshal.PtrToStringAnsi(errMsg);
            throw new Exception();
        }

        public static readonly TfliteErrorCallback 
            TfliteErrorHandlerThrowException = TfliteErrorHandler;

        private const string XamarinIOSObjectClassName = "Foundation.NSObject, Xamarin.iOS";

        static TfLiteInvoke()
        {
            var inXamarinIOS = Type.GetType(XamarinIOSObjectClassName) != null;
            if (inXamarinIOS)
            {
                TfLiteDllImportXamarinIOS.RedirectError(TfliteErrorHandlerThrowException);

                TfeFlatBufferModelBuildFromFile = TfLiteDllImportXamarinIOS.tfeFlatBufferModelBuildFromFile;
                TfeFlatBufferModelBuildFromBuffer = TfLiteDllImportXamarinIOS.tfeFlatBufferModelBuildFromBuffer;
                TfeFlatBufferModelRelease = TfLiteDllImportXamarinIOS.tfeFlatBufferModelRelease;
                TfeFlatBufferModelInitialized = TfLiteDllImportXamarinIOS.tfeFlatBufferModelInitialized;
                TfeFlatBufferModelCheckModelIdentifier = 
                    TfLiteDllImportXamarinIOS.tfeFlatBufferModelCheckModelIdentifier;
                TfeMemcpy = TfLiteDllImportXamarinIOS.tfeMemcpy;
                TfeInterpreterCreate = TfLiteDllImportXamarinIOS.tfeInterpreterCreate;
                TfeInterpreterCreateFromModel = TfLiteDllImportXamarinIOS.tfeInterpreterCreateFromModel;
                TfeInterpreterAllocateTensors = TfLiteDllImportXamarinIOS.tfeInterpreterAllocateTensors;
                TfeInterpreterInvoke = TfLiteDllImportXamarinIOS.tfeInterpreterInvoke;
                TfeInterpreterGetTensor = TfLiteDllImportXamarinIOS.tfeInterpreterGetTensor;
                TfeInterpreterTensorSize = TfLiteDllImportXamarinIOS.tfeInterpreterTensorSize;
                TfeInterpreterNodesSize = TfLiteDllImportXamarinIOS.tfeInterpreterNodesSize;
                TfeInterpreterGetInputSize = TfLiteDllImportXamarinIOS.tfeInterpreterGetInputSize;
                TfeInterpreterGetInput = TfLiteDllImportXamarinIOS.tfeInterpreterGetInput;
                TfeInterpreterGetInputName = TfLiteDllImportXamarinIOS.tfeInterpreterGetInputName;
                TfeInterpreterGetOutputSize = TfLiteDllImportXamarinIOS.tfeInterpreterGetOutputSize;
                TfeInterpreterGetOutput = TfLiteDllImportXamarinIOS.tfeInterpreterGetOutput;
                TfeInterpreterGetOutputName = TfLiteDllImportXamarinIOS.tfeInterpreterGetOutputName;
                TfeInterpreterRelease = TfLiteDllImportXamarinIOS.tfeInterpreterRelease;
                TfeInterpreterSetNumThreads = TfLiteDllImportXamarinIOS.tfeInterpreterSetNumThreads;
                TfeTensorGetType = TfLiteDllImportXamarinIOS.tfeTensorGetType;
                TfeTensorGetData = TfLiteDllImportXamarinIOS.tfeTensorGetData;
                TfeTensorGetQuantizationParams = TfLiteDllImportXamarinIOS.tfeTensorGetQuantizationParams;
                TfeTensorGetAllocationType = TfLiteDllImportXamarinIOS.tfeTensorGetAllocationType;
                TfeTensorGetByteSize = TfLiteDllImportXamarinIOS.tfeTensorGetByteSize;
                TfeTensorGetName = TfLiteDllImportXamarinIOS.tfeTensorGetName;
                TfeBuiltinOpResolverCreate = TfLiteDllImportXamarinIOS.tfeBuiltinOpResolverCreate;
                TfeBuiltinOpResolverRelease = TfLiteDllImportXamarinIOS.tfeBuiltinOpResolverRelease;
            }
            else
            {
                TfLiteDllImport.RedirectError(TfliteErrorHandlerThrowException);

                TfeFlatBufferModelBuildFromFile = TfLiteDllImport.tfeFlatBufferModelBuildFromFile;
                TfeFlatBufferModelBuildFromBuffer = TfLiteDllImport.tfeFlatBufferModelBuildFromBuffer;
                TfeFlatBufferModelRelease = TfLiteDllImport.tfeFlatBufferModelRelease;
                TfeFlatBufferModelInitialized = TfLiteDllImport.tfeFlatBufferModelInitialized;
                TfeFlatBufferModelCheckModelIdentifier = 
                    TfLiteDllImport.tfeFlatBufferModelCheckModelIdentifier;
                TfeMemcpy = TfLiteDllImport.tfeMemcpy;
                TfeInterpreterCreate = TfLiteDllImport.tfeInterpreterCreate;
                TfeInterpreterCreateFromModel = TfLiteDllImport.tfeInterpreterCreateFromModel;
                TfeInterpreterAllocateTensors = TfLiteDllImport.tfeInterpreterAllocateTensors;
                TfeInterpreterInvoke = TfLiteDllImport.tfeInterpreterInvoke;
                TfeInterpreterGetTensor = TfLiteDllImport.tfeInterpreterGetTensor;
                TfeInterpreterTensorSize = TfLiteDllImport.tfeInterpreterTensorSize;
                TfeInterpreterNodesSize = TfLiteDllImport.tfeInterpreterNodesSize;
                TfeInterpreterGetInputSize = TfLiteDllImport.tfeInterpreterGetInputSize;
                TfeInterpreterGetInput = TfLiteDllImport.tfeInterpreterGetInput;
                TfeInterpreterGetInputName = TfLiteDllImport.tfeInterpreterGetInputName;
                TfeInterpreterGetOutputSize = TfLiteDllImport.tfeInterpreterGetOutputSize;
                TfeInterpreterGetOutput = TfLiteDllImport.tfeInterpreterGetOutput;
                TfeInterpreterGetOutputName = TfLiteDllImport.tfeInterpreterGetOutputName;
                TfeInterpreterRelease = TfLiteDllImport.tfeInterpreterRelease;
                TfeInterpreterSetNumThreads = TfLiteDllImport.tfeInterpreterSetNumThreads;
                TfeTensorGetType = TfLiteDllImport.tfeTensorGetType;
                TfeTensorGetData = TfLiteDllImport.tfeTensorGetData;
                TfeTensorGetQuantizationParams = TfLiteDllImport.tfeTensorGetQuantizationParams;
                TfeTensorGetAllocationType = TfLiteDllImport.tfeTensorGetAllocationType;
                TfeTensorGetByteSize = TfLiteDllImport.tfeTensorGetByteSize;
                TfeTensorGetName = TfLiteDllImport.tfeTensorGetName;
                TfeBuiltinOpResolverCreate = TfLiteDllImport.tfeBuiltinOpResolverCreate;
                TfeBuiltinOpResolverRelease = TfLiteDllImport.tfeBuiltinOpResolverRelease;
            }
        }

        public static readonly TfLiteDelegates.tfeFlatBufferModelBuildFromFile TfeFlatBufferModelBuildFromFile;
        public static readonly TfLiteDelegates.tfeFlatBufferModelBuildFromBuffer TfeFlatBufferModelBuildFromBuffer;
        public static readonly TfLiteDelegates.tfeFlatBufferModelRelease TfeFlatBufferModelRelease;
        public static readonly TfLiteDelegates.tfeFlatBufferModelInitialized TfeFlatBufferModelInitialized;
        public static readonly TfLiteDelegates.tfeFlatBufferModelCheckModelIdentifier 
            TfeFlatBufferModelCheckModelIdentifier;

        public static readonly TfLiteDelegates.tfeMemcpy TfeMemcpy;

        public static readonly TfLiteDelegates.tfeInterpreterCreate TfeInterpreterCreate;

        public static readonly TfLiteDelegates.tfeInterpreterCreateFromModel TfeInterpreterCreateFromModel;

        public static readonly TfLiteDelegates.tfeInterpreterAllocateTensors TfeInterpreterAllocateTensors;

        public static readonly TfLiteDelegates.tfeInterpreterInvoke TfeInterpreterInvoke;

        public static readonly TfLiteDelegates.tfeInterpreterGetTensor TfeInterpreterGetTensor;

        public static readonly TfLiteDelegates.tfeInterpreterTensorSize TfeInterpreterTensorSize;

        public static readonly TfLiteDelegates.tfeInterpreterNodesSize TfeInterpreterNodesSize;

        public static readonly TfLiteDelegates.tfeInterpreterGetInputSize TfeInterpreterGetInputSize;

        public static readonly TfLiteDelegates.tfeInterpreterGetInput TfeInterpreterGetInput;

        public static readonly TfLiteDelegates.tfeInterpreterGetInputName TfeInterpreterGetInputName;

        public static readonly TfLiteDelegates.tfeInterpreterGetOutputSize TfeInterpreterGetOutputSize;

        public static readonly TfLiteDelegates.tfeInterpreterGetOutput TfeInterpreterGetOutput;

        public static readonly TfLiteDelegates.tfeInterpreterGetOutputName TfeInterpreterGetOutputName;

        public static readonly TfLiteDelegates.tfeInterpreterRelease TfeInterpreterRelease;

        public static readonly TfLiteDelegates.tfeInterpreterSetNumThreads TfeInterpreterSetNumThreads;

        public static readonly TfLiteDelegates.tfeTensorGetType TfeTensorGetType;

        public static readonly TfLiteDelegates.tfeTensorGetData TfeTensorGetData;

        public static readonly TfLiteDelegates.tfeTensorGetQuantizationParams TfeTensorGetQuantizationParams;

        public static readonly TfLiteDelegates.tfeTensorGetAllocationType TfeTensorGetAllocationType;

        public static readonly TfLiteDelegates.tfeTensorGetByteSize TfeTensorGetByteSize;

        public static readonly TfLiteDelegates.tfeTensorGetName TfeTensorGetName;

        public static readonly TfLiteDelegates.tfeBuiltinOpResolverCreate TfeBuiltinOpResolverCreate;

        public static readonly TfLiteDelegates.tfeBuiltinOpResolverRelease TfeBuiltinOpResolverRelease;
    }

    public static partial class TfLiteDelegates
    {
        public delegate IntPtr tfeFlatBufferModelBuildFromFile(string filename);

        public delegate IntPtr tfeFlatBufferModelBuildFromBuffer(IntPtr buffer, int bufferSize);

        public delegate void tfeFlatBufferModelRelease(ref IntPtr model);

        public delegate bool tfeFlatBufferModelInitialized(IntPtr model);

        public delegate bool tfeFlatBufferModelCheckModelIdentifier(IntPtr model);

        public delegate void tfeMemcpy(IntPtr dst, IntPtr src, int length);

        public delegate IntPtr tfeInterpreterCreate();

        public delegate IntPtr tfeInterpreterCreateFromModel(IntPtr model, IntPtr opResolver);

        public delegate Status tfeInterpreterAllocateTensors(IntPtr interpreter);

        public delegate Status tfeInterpreterInvoke(IntPtr interpreter);

        public delegate IntPtr tfeInterpreterGetTensor(IntPtr interpreter, int index);

        public delegate int tfeInterpreterTensorSize(IntPtr interpreter);

        public delegate int tfeInterpreterNodesSize(IntPtr interpreter);

        public delegate int tfeInterpreterGetInputSize(IntPtr interpreter);

        public delegate void tfeInterpreterGetInput(IntPtr interpreter, IntPtr input);

        public delegate IntPtr tfeInterpreterGetInputName(IntPtr interpreter, int index);

        public delegate int tfeInterpreterGetOutputSize(IntPtr interpreter);

        public delegate void tfeInterpreterGetOutput(IntPtr interpreter, IntPtr output);

        public delegate IntPtr tfeInterpreterGetOutputName(IntPtr interpreter, int index);

        public delegate void tfeInterpreterRelease(ref IntPtr interpreter);

        public delegate void tfeInterpreterSetNumThreads(IntPtr interpreter, int num_threads);

        public delegate DataType tfeTensorGetType(IntPtr tensor);

        public delegate IntPtr tfeTensorGetData(IntPtr tensor);

        public delegate void tfeTensorGetQuantizationParams(IntPtr tensor, ref QuantizationParams p);

        public delegate AllocationType tfeTensorGetAllocationType(IntPtr tensor);

        public delegate int tfeTensorGetByteSize(IntPtr tensor);

        public delegate IntPtr tfeTensorGetName(IntPtr tensor);

        public delegate IntPtr tfeBuiltinOpResolverCreate(ref IntPtr opResolver);

        public delegate void tfeBuiltinOpResolverRelease(ref IntPtr resolver);
    }
}
