using System;

namespace console.Extensions
{
    public static class ObservableExtension
    {
        public static IDisposable SubscribeConsole<T>(this IObservable<T> observable, string name = "") => 
            observable.Subscribe(new ConsoleObserver<T>(name));
    }
}