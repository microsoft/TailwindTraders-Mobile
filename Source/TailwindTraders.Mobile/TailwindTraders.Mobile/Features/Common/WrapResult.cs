namespace TailwindTraders.Mobile.Features.Common
{
    public struct WrapResult<T>
    {
        public WrapResult(T value, bool isSucceeded)
        {
            IsSucceded = isSucceeded;
            Result = value;
        }

        public static WrapResult<T> Succeded => new WrapResult<T>(default(T), true);

        public static WrapResult<T> Failed => new WrapResult<T>(default(T), false);

        public bool IsSucceded { get; private set; }

        public T Result { get; private set; }
    }
}
