using System;

namespace console.Common
{
    internal class WeakObserverProxy<T> : IObserver<T>
    {
        private IDisposable _subscriptionToSource;
        private readonly WeakReference<IObserver<T>> _weakObserver;

        public WeakObserverProxy(IObserver<T> observer) => _weakObserver = new WeakReference<IObserver<T>>(observer);

        public void OnNext(T value) => NotifyObserver(observer => observer.OnNext(value));
        public void OnError(Exception error) => NotifyObserver(observer => observer.OnError(error));
        public void OnCompleted() => NotifyObserver(observer => observer.OnCompleted());

        internal void SetSubscription(IDisposable subscriptionToSource) => _subscriptionToSource = subscriptionToSource;
        public IDisposable AsDisposable() => _subscriptionToSource;

        private void NotifyObserver(Action<IObserver<T>> action)
        {
            if (_weakObserver.TryGetTarget(out var observer))
                action(observer);
            else
                _subscriptionToSource.Dispose();
        }
    }
}