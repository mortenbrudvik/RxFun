using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using console.Extensions;

namespace console
{
    public class CreatingObservableSequences
    {
        public static void Run()
        {
            // Convert event to Observable
            IObservable<EventPattern<ConsoleCancelEventArgs>> cancel =
                 Observable.FromEventPattern<ConsoleCancelEventHandler, ConsoleCancelEventArgs>(
                     h => Console.CancelKeyPress += h,
                     h => Console.CancelKeyPress -= h);
            cancel.SubscribeConsole();
            
            // IEnumerable to Observable 
            NumbersAndThrow()
                .ToObservable()
                .SubscribeConsole();
            
            // Generating an observable loop
            var evenNumberObservable = Observable.Generate(
                0,
                i => i < 10,
                i => i + 1,
                i => i * 2);
            evenNumberObservable.SubscribeConsole();
            
            // Using Range
            Observable
                .Range(0, 10)
                .Select(i => i * 2)
                .SubscribeConsole();
            
            // Reading file
            Observable.Generate(
                    File.OpenText("TextFile.txt"),
                    s => !s.EndOfStream,
                    s => s,
                    s => s.ReadLine())
                .SubscribeConsole();
            
            // Reading file and calling dispose afterward
            Observable.Using(
                () => File.OpenText("TextFile.txt"),
                stream =>
                    Observable.Generate(
                        stream,
                        s => !s.EndOfStream,
                        s => s,
                        s => s.ReadLine()))
                .SubscribeConsole();
            
            // Single item observable
            Observable.Return("Hello!")
                .SubscribeConsole("Return");

            // pushes nothing and never comples
            Observable.Never<string>()
                .SubscribeConsole("Never");

            // Simulate a throw exception
            Observable.Throw<ApplicationException>(new ApplicationException("bad"))
                .SubscribeConsole();

            // pushes nothing and completes at once.
            Observable.Empty<string>()
                .SubscribeConsole();
        }
        
        static IEnumerable<int> NumbersAndThrow()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            throw new ApplicationException("Something bad happened");
            yield return 4;
        }
        
    }
}