//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;

namespace Emgu.TF.Util
{
   /// <summary>
   /// An Unmanaged Object is a disposable object with a Ptr property pointing to the unmanaged object
   /// </summary>
   public abstract class UnmanagedObject : DisposableObject
   {
      /// <summary>
      /// A pointer to the unmanaged object
      /// </summary>
      protected IntPtr ptr;

      /// <summary>
      /// Pointer to the unmanaged object
      /// </summary>
      public virtual IntPtr Ptr
      {
         get
         {
            return ptr;
         }
      }

      /// <summary>
      /// Implicit operator for IntPtr
      /// </summary>
      /// <param name="obj">The UnmanagedObject</param>
      /// <returns>The unmanaged pointer for this object</returns>
      public static implicit operator IntPtr(UnmanagedObject obj)
      {
         return obj == null ? IntPtr.Zero : obj.ptr;
      }
   }
}
