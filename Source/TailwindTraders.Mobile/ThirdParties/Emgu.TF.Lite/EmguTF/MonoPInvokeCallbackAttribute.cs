using System;

namespace Emgu.TF.Lite
{
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private readonly Type type;

        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }
}
