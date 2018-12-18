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
    /// The tensorflow lite interpreter.
    /// </summary>
    public class Interpreter : Emgu.TF.Util.UnmanagedObject
    {
        /// <summary>
        /// Create an interpreter from a flatbuffer model
        /// </summary>
        /// <param name="flatBufferModel">The flat buffer model.</param>
        /// <param name="resolver">An instance that implements the Resolver interface which maps custom op names 
        /// and builtin op codes to op registrations.</param>
        public Interpreter(FlatBufferModel flatBufferModel, IOpResolver resolver)
        {
            ptr = TfLiteInvoke.TfeInterpreterCreateFromModel(flatBufferModel.Ptr, resolver.OpResolverPtr);
        }

        /// <summary>
        /// Update allocations for all tensors. This will redim dependent tensors using
        /// the input tensor dimensionality as given. This is relatively expensive.
        /// If you know that your sizes are not changing, you need not call this.
        /// </summary>
        /// <returns>Status of success or failure.</returns>
        public Status AllocateTensors()
        {
            return TfLiteInvoke.TfeInterpreterAllocateTensors(ptr);
        }

        /// <summary>
        /// Invoke the interpreter (run the whole graph in dependency order).
        /// </summary>
        /// <returns>Status of success or failure.</returns>
        /// <remarks>It is possible that the interpreter is not in a ready state
        /// to evaluate (i.e. if a ResizeTensor() has been performed without an
        /// AllocateTensors().
        /// </remarks>
        public Status Invoke()
        {
            return TfLiteInvoke.TfeInterpreterInvoke(ptr);
        }

        public void SetNumThreads(int num_threads)
        {
            TfLiteInvoke.TfeInterpreterSetNumThreads(ptr, num_threads);
        }

        /// <summary>
        /// Get the number of tensors in the model.
        /// </summary>
        public int TensorSize
        {
            get
            {
                return TfLiteInvoke.TfeInterpreterTensorSize(ptr);
            }
        }

        /// <summary>
        /// Get the number of ops in the model.
        /// </summary>
        public int NodeSize
        {
            get
            {
                return TfLiteInvoke.TfeInterpreterNodesSize(ptr);
            }
        }

        /// <summary>
        /// Get a tensor data structure.
        /// </summary>
        /// <param name="index">The index of the tensor</param>
        /// <returns>The tensor in the specific index</returns>
        public Tensor GetTensor(int index)
        {
            return new Tensor(TfLiteInvoke.TfeInterpreterGetTensor(ptr, index), false);
        }

        /// <summary>
        /// Get the list of tensor index of the inputs tensors.
        /// </summary>
        /// <returns>The list of tensor index of the inputs tensors.</returns>
        public int[] GetInput()
        {
            int size = TfLiteInvoke.TfeInterpreterGetInputSize(ptr);
            int[] input = new int[size];
            GCHandle handle = GCHandle.Alloc(input, GCHandleType.Pinned);
            TfLiteInvoke.TfeInterpreterGetInput(ptr, handle.AddrOfPinnedObject());
            handle.Free();
            return input;
        }

        /// <summary>
        /// Get the list of tensor index of the outputs tensors.
        /// </summary>
        /// <returns>The list of tensor index of the outputs tensors.</returns>
        public int[] GetOutput()
        {
            int size = TfLiteInvoke.TfeInterpreterGetOutputSize(ptr);
            int[] output = new int[size];
            GCHandle handle = GCHandle.Alloc(output, GCHandleType.Pinned);
            TfLiteInvoke.TfeInterpreterGetOutput(ptr, handle.AddrOfPinnedObject());
            handle.Free();
            return output;
        }

        /// <summary>
        /// Return the name of a given input
        /// </summary>
        /// <param name="index">The input tensor index</param>
        /// <returns>The name of the input tesnsor at the index</returns>
        public string GetInputName(int index)
        {
            IntPtr namePtr = TfLiteInvoke.TfeInterpreterGetInputName(ptr, index);
            return Marshal.PtrToStringAnsi(namePtr);
        }

        /// <summary>
        /// Return the name of a given output
        /// </summary>
        /// <param name="index">The output tensor index</param>
        /// <returns>The name of the output tesnsor at the index</returns>
        public string GetOutputName(int index)
        {
            IntPtr namePtr = TfLiteInvoke.TfeInterpreterGetOutputName(ptr, index);
            return Marshal.PtrToStringAnsi(namePtr);
        }

        /// <summary>
        /// Release all the unmanaged memory associated with this interpreter
        /// </summary>
        protected override void DisposeObject()
        {
            if (ptr != IntPtr.Zero)
            {
                TfLiteInvoke.TfeInterpreterRelease(ref ptr);
            }
        }
    }
}
