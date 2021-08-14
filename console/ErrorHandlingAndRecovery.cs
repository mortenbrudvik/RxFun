using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using console.Extensions;

namespace console
{
    public static class ErrorHandlingAndRecovery
    {
        public static void Run()
        {
            // Catch operator
            var weatherSimulationResults = Observable.Throw<WeatherSimulation>(new OutOfMemoryException());
            weatherSimulationResults
                .Catch((OutOfMemoryException ex) =>
                {
                    Console.Out.WriteLine("handling OOM exception");
                    return Observable.Empty<WeatherSimulation>();
                })
                .SubscribeConsole("catch (source throws)");
            weatherSimulationResults
                .Catch(Observable.Empty<WeatherSimulation>())
                .SubscribeConsole("Catch (Handling all exception types)");

            // OnErrorResumeNext - concat the second observable whe the first completes or throws
            var stationA = Observable.Throw<WeatherReport>(new OutOfMemoryException());
            var stationB = Observable.Return<WeatherReport>(new WeatherReport()
            {
                Station = "B",
                Temperature = 20.0
            });
            stationA
                .OnErrorResumeNext(stationB)
                .SubscribeConsole("OnErrorResumeNext(source throws)");
            stationB
                .OnErrorResumeNext(stationB)
                .SubscribeConsole("OnErrorResumeNext(source complete)");

            // Retry - Cold observables
            var weatherStation = Observable.Throw<WeatherReport>(new OutOfMemoryException());
            weatherStation
                .Log()
                .Retry(5)
                .SubscribeConsole("Retry");

            // The Using operator
            File.Delete("sensor-data.txt");
            var sensorData = Observable.Range(1, 3)
                .Select(x => new SensorData(x + ""));
            Observable.Using(() => new StreamWriter("sensor-data.txt"),
                    writer => sensorData.Do(
                        x => writer.WriteLine(x.Data)))
                .SubscribeConsole("sensor");

            // Disposes for any termination
            var subject = new Subject<int>();
            var observable = Observable.Using(
                () => Disposable.Create(
                    () => Console.WriteLine("DISPOSED")),
                _ => subject);
            observable.SubscribeConsole("Disposed when completed");
            subject.OnCompleted();
            observable.SubscribeConsole("Disposed when error occurs");
            subject.OnError(new Exception("error"));
            observable.SubscribeConsole("Disposed when subscription disposed");
            subject.Dispose();

            // Finally
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(3)
                .Finally(() => Console.Out.WriteLine("Finally code"))
                .SubscribeConsole();
            Observable.Throw<Exception>(new Exception("error"))
                .Finally(() => Console.Out.WriteLine("error"))
                .SubscribeConsole();
            var subject2 = new Subject<int>();
            var subscription = subject2.AsObservable()
                .Finally(() => Console.WriteLine("Finally Code"))
                .SubscribeConsole();
            subscription.Dispose();

            // Weak reference example for a object
            var obj = new object();
            var weak = new WeakReference(obj);
            GC.Collect();
            Console.Out.WriteLine($"IsAlive: {weak.IsAlive} obj!=null is {obj != null}");
            obj = null;
            GC.Collect();
            Console.Out.WriteLine($"IsAlive: {weak.IsAlive}");

            // Weak reference for observer (NB! This is currently not working as expected...)
            var weakSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
                .AsWeakObservable()
                .SubscribeConsole("Interval");
            Console.Out.WriteLine("Collecting");
            GC.Collect();
            Thread.Sleep(2000);

            GC.KeepAlive(weakSubscription);
            Console.Out.WriteLine("Done sleeping");
            Console.Out.WriteLine("Collecting");

            weakSubscription = null;
            GC.Collect();
            Thread.Sleep(2000);
            Console.Out.WriteLine("Done sleeping");
        }
    }
}