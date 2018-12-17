//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Emgu.TF.Lite
{
    /// <summary>
    /// Abstract interface that returns TfLiteRegistrations given op codes or custom
    /// op names. This is the mechanism that ops being referenced in the flatbuffer
    /// model are mapped to executable function pointers (TfLiteRegistrations)
    /// </summary>
    public interface IOpResolver
    {
        /// <summary>
        /// Pointer to the native OpResolver object.
        /// </summary>
        IntPtr OpResolverPtr { get; }
    }
}
