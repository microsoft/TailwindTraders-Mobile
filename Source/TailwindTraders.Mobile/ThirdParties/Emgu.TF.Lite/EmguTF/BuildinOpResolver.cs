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
    /// The default tensor flow lite buildin op resolver.
    /// </summary>
    public class BuildinOpResolver : Emgu.TF.Util.UnmanagedObject, IOpResolver
    {
        private IntPtr opResolverPtr;
        
        /// <summary>
        /// Create a default buildin op resolver.
        /// </summary>
        public BuildinOpResolver()
        {
            ptr = TfLiteInvoke.tfeBuiltinOpResolverCreate(ref opResolverPtr);
        }
        
        IntPtr IOpResolver.OpResolverPtr
        {
            get
            {
                return opResolverPtr;
            }
        }

        /// <summary>
        /// Release all the unmanaged memory associated with this model
        /// </summary>
        protected override void DisposeObject()
        {
            if (ptr != IntPtr.Zero)
            {
                TfLiteInvoke.tfeBuiltinOpResolverRelease(ref ptr);
                opResolverPtr = IntPtr.Zero;
            }
        }
    }

    /// <summary>
    /// Class that provide access to native tensorflow lite functions
    /// </summary>
    public static partial class TfLiteInvoke
    {
        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeBuiltinOpResolverCreate(ref IntPtr opResolver);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeBuiltinOpResolverRelease(ref IntPtr resolver);
    }
}
