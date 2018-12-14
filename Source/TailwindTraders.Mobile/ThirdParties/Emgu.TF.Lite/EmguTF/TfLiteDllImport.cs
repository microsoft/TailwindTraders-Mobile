//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Emgu.TF.Lite
{
    public static partial class TfLiteDllImport
    {
        public const string ExternLibrary = "tfliteextern";

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention, EntryPoint = "tfeRedirectError")]
        internal static extern void RedirectError(TfLiteInvoke.TfliteErrorCallback errorHandler);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr 
            tfeFlatBufferModelBuildFromFile([MarshalAs(Constants.StringMarshalType)] string filename);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeFlatBufferModelBuildFromBuffer(IntPtr buffer, int bufferSize);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeFlatBufferModelRelease(ref IntPtr model);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        [return: MarshalAs(Constants.BoolMarshalType)]
        internal static extern bool tfeFlatBufferModelInitialized(IntPtr model);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        [return: MarshalAs(Constants.BoolMarshalType)]
        internal static extern bool tfeFlatBufferModelCheckModelIdentifier(IntPtr model);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeMemcpy(IntPtr dst, IntPtr src, int length);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterCreate();

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterCreateFromModel(IntPtr model, IntPtr opResolver);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern Status tfeInterpreterAllocateTensors(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern Status tfeInterpreterInvoke(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterGetTensor(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern int tfeInterpreterTensorSize(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern int tfeInterpreterNodesSize(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern int tfeInterpreterGetInputSize(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeInterpreterGetInput(IntPtr interpreter, IntPtr input);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterGetInputName(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern int tfeInterpreterGetOutputSize(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeInterpreterGetOutput(IntPtr interpreter, IntPtr output);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterGetOutputName(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeInterpreterRelease(ref IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeInterpreterSetNumThreads(IntPtr interpreter, int num_threads);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern DataType tfeTensorGetType(IntPtr tensor);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeTensorGetData(IntPtr tensor);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeTensorGetQuantizationParams(IntPtr tensor, ref QuantizationParams p);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern AllocationType tfeTensorGetAllocationType(IntPtr tensor);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern int tfeTensorGetByteSize(IntPtr tensor);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeTensorGetName(IntPtr tensor);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern IntPtr tfeBuiltinOpResolverCreate(ref IntPtr opResolver);

        [DllImport(ExternLibrary, CallingConvention = Constants.TFCallingConvention)]
        internal static extern void tfeBuiltinOpResolverRelease(ref IntPtr resolver);
    }
}
