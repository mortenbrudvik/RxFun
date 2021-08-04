using System;
using System.Reactive;
using System.Reactive.Linq;
using console.Extensions;

namespace console
{
    public static class ControllingObservableObserverLifetime
    {
        public static void Run()
        {
            // Creating an observer instance
            var observer = Observer.Create<string>(x => Console.Out.WriteLine(x));
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(x => "X=" + x)
                .Take(5)
                .Subscribe(observer);
            Observable.Interval(TimeSpan.FromSeconds(2))
                .Select(x => "Y=" + x)
                .Take(5)
                .Subscribe(observer);
            
            // Delay subscription
            Console.Out.WriteLine($"Creating observable pipeline {DateTime.Now}");
            Observable.Range(1, 5)
                .Timestamp()
                .DelaySubscription(TimeSpan.FromSeconds(5))
                .SubscribeConsole();
            
            // Stop emitting notifications at a scheduled time
            Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
                .Select(x => DateTime.Now)
                .TakeUntil(DateTimeOffset.Now.AddSeconds(5))
                .SubscribeConsole("TakeUntil(time");
            
            // Discard items when another observable emits
            Observable.Timer(DateTime.Now, TimeSpan.FromSeconds(1))
                .Select(x => DateTime.Now)
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(5)))
                .SubscribeConsole("TakeUntil(observable)");
            
            // Skipping notifications
            Observable.Range(1, 5)
                .Skip(2)
                .SubscribeConsole("Skip");
            
            // Taking or skipping when a condition is met
            Observable.Range(1, 10)
                .SkipWhile(x => x < 2)
                .TakeWhile(x => x < 7)
                .SubscribeConsole();
            
            // Resubscribing 
            Observable.Range(1, 3)
                .Repeat(2)
                .SubscribeConsole("Repeat(2)");
            
            // Adding side effect - do operator
            Observable.Range(1, 5)
                .Do(x => Console.Out.WriteLine($"{x} was emitted"))
                .Where(x => x % 2 == 0)
                .Do(x => { Console.Out.WriteLine($"{x} survived the Where()"); })
                .Select(x => x * 3)
                .SubscribeConsole("final");
            
            // Log operator (extension)
            Observable.Range(1, 5)
                .Log("range")
                .Where(x => x % 2 == 0)
                .Log("where")
                .Select(x => x * 3)
                .SubscribeConsole();
        }
    }
}