using System;
using System.Collections.Generic;
using System.Text;

namespace TailwindTraders.Mobile.Helpers
{
    public class EventArgsT<T> : EventArgs
    {
        public T Value { get; }

        public EventArgsT(T val) => Value = val;
    }
}
