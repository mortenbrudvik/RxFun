using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using console.Extensions;

using static console.NumberGenerator;

namespace console
{
    public static class PartitioningAndCombining
    {
        public static void Run()
        {
            // Zip - Combines at same index
            var sensor1 = GenerateRandom(10, 60, 70, _ => 1000);
            var sensor2 = GenerateRandom(10, 60, 70, _ => 1000);
            sensor1.Zip(sensor2, (t1, t2) => $"s1: {t1}, s2: {t2} Avg: {(t1 + t2) / 2}" )
                .SubscribeConsole();
            
            // Combining the latest emitted values
            var heartRate = new Subject<int>();
            var speed = new Subject<int>();
            speed.CombineLatest(heartRate, (s, h) => $"Heart:{h} Speed:{s}")
                .SubscribeConsole("Metrics");
            heartRate.OnNext(150);
            heartRate.OnNext(151);
            heartRate.OnNext(152);
            speed.OnNext(30);    
            speed.OnNext(31);    
            heartRate.OnNext(153);
            heartRate.OnNext(154);
            
            // Concatenating Observables
            var messages1 = Task.Delay(10).ContinueWith(_ => new[] { "m1_1", "m1_2" });
            var messages2 = Task.FromResult(new[] { "m2_1", "m2_2" });
            Observable.Concat(messages1.ToObservable(), messages2.ToObservable())
                .SelectMany(m => m)
                .SubscribeConsole("Concat messages");

            // Merging observables
            var msgs1 = Task.Delay(100).ContinueWith(_ => new[] { "m1_1", "m1_2" });
            var msgs2 = Task.FromResult(new[] { "m2_1", "m2_2" });
            Observable.Merge(msgs1.ToObservable(), msgs2.ToObservable())
                .SelectMany(m => m)
                .SubscribeConsole("Merged messages");

            // Dynamic concatenation
            var texts = new[] { "Hello", "World" }.ToObservable();
            texts.Select(txt => Observable.Return(txt + "-Result"))
                .Merge()
                .SubscribeConsole();
            
            // Switching to most recent
            var textSubject = new Subject<string>();
            var texts2 = textSubject.AsObservable();
            texts2.Select(txt => Observable.Return(txt + "-Result")
                    .Delay(TimeSpan.FromMilliseconds(txt == "R1" ? 10 : 0)))
                .Switch()
                .SubscribeConsole("Merging from observables");
            textSubject.OnNext("R1");
            textSubject.OnNext("R2");
            Thread.Sleep(20);
            textSubject.OnNext("R3");
            
            // Switching to first to emit
            var server1 = Observable.Interval(TimeSpan.FromSeconds(2))
                .Select(i => "Server1-" + i);
            var server2 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(i => "Server2-" + i);
            Observable.Amb(server1, server2)
                .Take(3)
                .SubscribeConsole("Amb");
            
            
        }
    }
}