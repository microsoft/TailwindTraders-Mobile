//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Emgu.TF.Util
{
   /// <summary>
   /// A Pinnned array of the specific type
   /// </summary>
   /// <typeparam name="T">The type of the array</typeparam>
   public class PinnedArray<T> : DisposableObject
   {
      private T[] array;
      private GCHandle handle;

      /// <summary>
      /// Create a Pinnned array of the specific type
      /// </summary>
      /// <param name="size">The size of the array</param>
      public PinnedArray(int size)
      {
         array = new T[size];
         handle = GCHandle.Alloc(array, GCHandleType.Pinned);
      }

      /// <summary>
      /// Get the address of the pinned array
      /// </summary>
      /// <returns>A pointer to the address of the the pinned array</returns>
      public IntPtr AddrOfPinnedObject()
      {
         return handle.AddrOfPinnedObject();
      }

      /// <summary>
      /// Get the array
      /// </summary>
      public T[] Array
      {
         get
         {
            return array;
         }
      }

      /// <summary>
      /// Release the GCHandle
      /// </summary>
      protected override void ReleaseManagedResources()
      {
         handle.Free();
      }

      /// <summary>
      /// Disposed the unmanaged data
      /// </summary>
      protected override void DisposeObject()
      {
      }
   }
}