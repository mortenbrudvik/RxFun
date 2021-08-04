using System;
using System.Reactive.Linq;

namespace console.Extensions
{
    public static class ObservableExtension
    {
        public static IDisposable SubscribeConsole<T>(this IObservable<T> observable, string name = "") => 
            observable.Subscribe(new ConsoleObserver<T>(name));
        
        public static IObservable<T> Log<T>(this IObservable<T> observable, string message = "")
        {
            return observable.Do(
                x => Console.WriteLine("{0} - OnNext({1})", message, x),
                ex =>
                {
                    Console.WriteLine("{0} - OnError:", message);
                    Console.WriteLine("\t {0}", ex);
                },
                () => Console.WriteLine("{0} - OnCompleted()", message));
        }
    }
}