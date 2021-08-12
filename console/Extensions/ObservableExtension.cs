using System;
using System.Reactive.Linq;
using System.Threading;

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

        public static IObservable<T> LogWithThread<T>(this IObservable<T> observable, string msg = "") =>
            Observable.Defer(() =>
            {
                Console.WriteLine("{0} Subscription happened on Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                return observable.Do(
                    x => Console.WriteLine("{0} - OnNext({1}) Thread: {2}", msg, x, Thread.CurrentThread.ManagedThreadId),
                    ex =>
                    {
                        Console.Out.WriteLine($"{msg} - OnError Thread:{Thread.CurrentThread.ManagedThreadId}");
                        Console.Out.WriteLine($"\t {ex}");
                    },
                    () => Console.Out.WriteLine($"{msg} Thread:{Thread.CurrentThread.ManagedThreadId}"));});
    }
}