using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using console.Extensions;

namespace console
{
    public static class ControllingTheObservableTemperature
    {
        public static async void Run()
        {
            // Broadcasting with Subject<t>
            var subject = new Subject<int>();
            subject.SubscribeConsole("First");
            subject.SubscribeConsole("Second");
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();
            
            // Broadcast by subscribing to two observables (NB! When first complete than also subject complete.)  
            var subject2 = new Subject<string>();
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(x => "First " + x)
                .Take(5)
                .Subscribe(subject2);
            Observable.Interval(TimeSpan.FromSeconds(2))
                .Select(x => "Second " + x)
                .Take(5)
                .Subscribe(subject2);
            subject2.SubscribeConsole();
            
            // Emits the last stored value on completion. Previous stored values will not be emitted
            var tcs = new TaskCompletionSource<bool>();
            var task = tcs.Task;
            var subject3 = new AsyncSubject<bool>();
            task.ContinueWith(t =>
            {
                switch (t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        subject3.OnNext(t.Result);
                        subject3.OnCompleted();
                        break;
                    case TaskStatus.Canceled:
                        subject3.OnError(t.Exception.InnerException);
                        break;
                    case TaskStatus.Faulted:
                        subject3.OnError(new TaskCanceledException());
                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            tcs.SetResult(true);
            subject3.SubscribeConsole();
            
            // Preserving the latest state for one value
            var connection = new BehaviorSubject<string>("Disconnected");
            connection.SubscribeConsole("first");
            connection.OnNext("Connected");
            connection.SubscribeConsole("second");
            await Console.Out.WriteLineAsync("Connection is: " + connection.Value);
                
            // Catching a sequence within a given time window
            var heartRate = Observable.Range(70, 5);
            var heartMonitor = new ReplaySubject<int>(20, TimeSpan.FromMinutes(2));
            heartRate.Subscribe(heartMonitor);
            heartMonitor.SubscribeConsole("heartRate");
            
            // Cold observables
            var coldObservable = Observable.Create<string>(async o =>
            {
                o.OnNext("Hello");
                await Task.Delay(TimeSpan.FromSeconds(1));
                o.OnNext("Rx");
            });
            coldObservable.SubscribeConsole("o1");
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            coldObservable.SubscribeConsole("o2");
            
            // Turning a cold observable hot. Simple Publish
            var coldObservable2 = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var connectableObservable = coldObservable2.Publish(); // does not emit when subscribed
            connectableObservable.SubscribeConsole("first");
            connectableObservable.SubscribeConsole("second");
            connectableObservable.Connect(); // start emitting 
            Thread.Sleep(2000);
            connectableObservable.SubscribeConsole("third");
            
            // Reusing the observable to create a new observable
            int i = 0;
            var numbers = Observable.Range(1, 5).Select(_ => i++); // Causes side effect - are called twice
            numbers
                .Zip(numbers, (a, b) => a + b)
                .SubscribeConsole("zipped");

            int i2 = 0;
            var numbers2 = Observable.Range(1, 5).Select(_ => i2++); 
            var publishedZip = numbers2.Publish(published => // defers the connect until subscription happen
                published.Zip(published, (a, b) => a + b));
            publishedZip.SubscribeConsole("publishedZip");
            
            // PublishLast
            var connectableObs = Observable.Timer(TimeSpan.FromSeconds(5))
                .Select(_ => "Rx")
                .PublishLast();
            connectableObs.SubscribeConsole("First");
            connectableObs.SubscribeConsole("Second");
            connectableObs.Connect();
            Thread.Sleep(6000);
            connectableObs.SubscribeConsole("Third");
            
            // Reconnect
            var tickTok = Observable.Interval(TimeSpan.FromSeconds(1))
                .Publish();
            var sub = tickTok.Connect();
            tickTok.SubscribeConsole("tickTok");
            Thread.Sleep(1000);
            await Console.Out.WriteLineAsync("Dispose current and reconnect");
            sub.Dispose();
            Thread.Sleep(3000);
            tickTok.Connect();
            
            // Automatic disconnection - using RefCount
            var publishedObservable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Do(x => Console.WriteLine("Generating {0}", x))
                .Publish()
                .RefCount();
            var s1 = publishedObservable.SubscribeConsole("First");
            var s2 = publishedObservable.SubscribeConsole("Second");
            Thread.Sleep(3000);
            s1.Dispose();
            Thread.Sleep(3000);
            s2.Dispose();
       
            // Cooling hot observable
            var po = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(5)
                .Replay(2);
            po.Connect();
            po.SubscribeConsole("First");
            Thread.Sleep(3000);
            po.SubscribeConsole("Second");
            
            
        }
        
    }
}