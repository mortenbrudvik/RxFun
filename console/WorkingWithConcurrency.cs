using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using console.Extensions;

namespace console
{
    public static class WorkingWithConcurrency
    {
        public static void Run()
        {
                        // Scheduler
            NewThreadScheduler.Default
                .Schedule(
                    Unit.Default,
                    TimeSpan.FromSeconds(2),
                    (scheduler, _) =>
                    {
                        Console.Out.WriteLine($"Hello World, Now: {scheduler.Now}");
                        return Disposable.Empty;
                    });
            
            // Scheduler - interval
            Func<IScheduler, int, IDisposable> action = null;
            action = (scdlr, callNumber) =>
            {
                Console.Out.WriteLine($"Hello {callNumber}, Now: {scdlr.Now}, Thread: {Thread.CurrentThread.ManagedThreadId}");
                return scdlr.Schedule(callNumber + 1, TimeSpan.FromSeconds(2), action);
            };
            NewThreadScheduler.Default.Schedule(0, TimeSpan.FromSeconds(2), action);
            
            // Run on current thread (will block until completed)
            Console.Out.WriteLine($"Before - Thread: {Thread.CurrentThread.ManagedThreadId}");
            Observable.Interval(TimeSpan.FromSeconds(1), CurrentThreadScheduler.Instance)
                .Take(3)
                .Subscribe(x => Console.Out.WriteLine($"Inside - Thread: {Thread.CurrentThread.ManagedThreadId}"));
            
            // Show time interval between emits 
            Observable.Timer(TimeSpan.FromSeconds(1))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(2)))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(4)))
                .TimeInterval()
                .SubscribeConsole("Time from last heartbeat");
            
            // Timeout
            Observable.Timer(TimeSpan.FromSeconds(1))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(1)))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(4)))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(4)))
                .Timeout(TimeSpan.FromSeconds(3))
                .SubscribeConsole("Timeout");
            
            // Delay
            Observable.Timer(TimeSpan.FromSeconds(1))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(1)))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(4)))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(4)))
                .Timestamp()
                .Delay(TimeSpan.FromSeconds(2))
                .Take(5)
                .SubscribeConsole("Delay");
            
            // Throttle
            Observable.Return("Update A")
                .Concat(Observable.Timer(TimeSpan.FromSeconds(2)).Select(_ => "Update B"))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => "Update C"))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => "Update D"))
                .Concat(Observable.Timer(TimeSpan.FromSeconds(3)).Select(_ => "Update E"))
                .Throttle(TimeSpan.FromSeconds(2))
                .SubscribeConsole("Throttle");
            
            // Sample at interval
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Sample(TimeSpan.FromSeconds(3.5))
                .Take(3)
                .SubscribeConsole("Sample");
            
            // Controlling Threads with SubscribeOn and ObserveOn
            Enumerable.Range(0, 6).ToObservable()
                .Take(3).LogWithThread("A").Where(x => x % 2 == 0).LogWithThread("B")
                .SubscribeOn(NewThreadScheduler.Default).LogWithThread("C")
                .Select(x => x * x).LogWithThread("D")
                .ObserveOn(TaskPoolScheduler.Default).LogWithThread("E")
                .SubscribeConsole("squares by time");
        }
    }
}