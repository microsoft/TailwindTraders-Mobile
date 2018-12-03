namespace TailwindTraders.Mobile.Features.Common
{
    public abstract class BaseStateAwareViewModel<T> : BaseViewModel
        where T : struct
    {
        private T currentState;

        public T CurrentState
        {
            get => currentState;
            set => SetAndRaisePropertyChanged(ref currentState, value);
        }
    }
}
