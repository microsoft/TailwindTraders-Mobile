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
    /// An RAII object that represents a read-only tflite model, copied from disk,
    /// or mmapped. This uses flatbuffers as the serialization format.
    /// </summary>   
    public class FlatBufferModel : Emgu.TF.Util.UnmanagedObject
    {
        private byte[] buffer = null;
        private GCHandle handle;

        /// <summary>
        /// Builds a model based on a file.
        /// </summary>   
        /// <param name="filename">The name of the file where the FlatBufferModel will be loaded from.</param>
        public FlatBufferModel(string filename)
        {
            ptr = TfLiteInvoke.TfeFlatBufferModelBuildFromFile(filename);
        }

        /// <summary>
        /// Builds a model based on a pre-loaded flatbuffer.
        /// </summary>   
        /// <param name="buffer">The buffer where the FlatBufferModel will be loaded from.</param>
        public FlatBufferModel(byte[] buffer)
        {
            this.buffer = new byte[buffer.Length];
            Array.Copy(buffer, this.buffer, this.buffer.Length);
            handle = GCHandle.Alloc(this.buffer, GCHandleType.Pinned);
            try
            {
                ptr = TfLiteInvoke.TfeFlatBufferModelBuildFromBuffer(handle.AddrOfPinnedObject(), buffer.Length);
            }
            catch
            {
                handle.Free();
                this.buffer = null;
                throw;
            }
        }

        /// <summary>
        /// Returns true if the model is initialized
        /// </summary>   
        public bool Initialized
        {
            get
            {
                return TfLiteInvoke.TfeFlatBufferModelInitialized(ptr);
            }
        }

        /// <summary>
        /// Returns true if the model identifier is correct (otherwise false and
        /// reports an error).
        /// </summary> 
        public bool CheckModelIdentifier()
        {
            return TfLiteInvoke.TfeFlatBufferModelCheckModelIdentifier(ptr);
        }

        /// <summary>
        /// Release all the unmanaged memory associated with this model
        /// </summary>
        protected override void DisposeObject()
        {
            if (ptr != IntPtr.Zero)
            {
                TfLiteInvoke.TfeFlatBufferModelRelease(ref ptr);
            }

            if (buffer != null)
            {
                handle.Free();
                buffer = null;
            }
        }
    }
}
