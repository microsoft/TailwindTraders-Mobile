//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Emgu.TF.Lite
{
    public static partial class TfLiteInvoke
    {
        [MonoPInvokeCallback(typeof(TfliteErrorCallback))]
        private static int TfliteErrorHandler(
           int status,
           IntPtr errMsg)
        {
            // TODO
            // String msg = Marshal.PtrToStringAnsi(errMsg);
            throw new Exception();
        }

        /// <summary>
        /// Define the funtional interface for the error callback
        /// </summary>
        /// <param name="status">The status code</param>
        /// <param name="errMsg">The pointer to the error message</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(TFCallingConvention)]
        public delegate int TfliteErrorCallback(int status, IntPtr errMsg);

        /// <summary>
        /// Redirect tensorflow lite error.
        /// </summary>
        /// <param name="errorHandler">The error handler</param>
        [DllImport(ExternLibrary, CallingConvention = TFCallingConvention, EntryPoint = "tfeRedirectError")]
        public static extern void RedirectError(TfliteErrorCallback errorHandler);

        /// <summary>
        /// The Tensorflow native api calling convention
        /// </summary>
        public const CallingConvention TFCallingConvention = CallingConvention.Cdecl;

        /// <summary>
        /// The string marshal type
        /// </summary>
        public const UnmanagedType StringMarshalType = UnmanagedType.LPStr;

        /// <summary>
        /// Represent a bool value in C++
        /// </summary>
        public const UnmanagedType BoolMarshalType = UnmanagedType.U1;

        /// <summary>
        /// Represent a int value in C++
        /// </summary>
        public const UnmanagedType BoolToIntMarshalType = UnmanagedType.Bool;

        /// <summary>
        /// Static Constructor to setup tensorflow environment
        /// </summary>
        static TfLiteInvoke()
        {
            // Use the custom error handler
            RedirectError(TfliteErrorHandlerThrowException);
        }

        /// <summary>
        /// The default error handler for tensorflow lite
        /// </summary>
        public static readonly TfliteErrorCallback TfliteErrorHandlerThrowException = 
            (TfliteErrorCallback)TfliteErrorHandler;

        [DllImport(ExternLibrary, CallingConvention = TFCallingConvention)]
        internal static extern void tfeMemcpy(IntPtr dst, IntPtr src, int length);
    }

    /// <summary>
    /// Tensorflow lite status
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Ok
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Error
        /// </summary>
        Error = 1,
    }
}
