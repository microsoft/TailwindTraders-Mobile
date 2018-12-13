//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Emgu.TF.Util;

namespace Emgu.TF.Lite
{
    /// <summary>
    /// DynamicBuffer holds temporary buffer that will be used to create a dynamic tensor. 
    /// </summary>
    public class DynamicBuffer : Emgu.TF.Util.UnmanagedObject
    {
        /// <summary>
        /// Create a new dynamic buffer.
        /// </summary>
        public DynamicBuffer()
        {
            ptr = TfLiteInvoke.tfeDynamicBufferCreate();
        }

        /// <summary>
        /// Add string to dynamic buffer by resizing the buffer and copying the data.
        /// </summary>
        /// <param name="str">The string to add to the dynamic buffer</param>
        public void AddString(string str)
        {
            byte[] rawString = Encoding.ASCII.GetBytes(str);
            GCHandle handle = GCHandle.Alloc(rawString, GCHandleType.Pinned);
            TfLiteInvoke.tfeDynamicBufferAddString(ptr, handle.AddrOfPinnedObject(), rawString.Length);
            handle.Free();
        }

        /// <summary>
        /// Fill content into a string tensor.
        /// </summary>
        /// <param name="tensor">The string tensor</param>
        public void WriteToTensor(Tensor tensor)
        {
            TfLiteInvoke.tfeDynamicBufferWriteToTensor(ptr, tensor);
        }
        
        /// <summary>
        /// Release all the unmanaged memory associated with this model
        /// </summary>
        protected override void DisposeObject()
        {
            if (ptr != IntPtr.Zero)
            {
                TfLiteInvoke.tfeDynamicBufferRelease(ref ptr);
            }
        }
    }

    public static partial class TfLiteInvoke
    {
        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeDynamicBufferCreate();

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeDynamicBufferRelease(ref IntPtr buffer);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeDynamicBufferAddString(IntPtr buffer, IntPtr str, int len);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeDynamicBufferWriteToTensor(IntPtr buffer, IntPtr tensor);
    }
}
