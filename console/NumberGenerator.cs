using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static System.Threading.Tasks.Task;

namespace console
{
    public static class NumberGenerator
    {
        public static IObservable<int> GeneratePrimes(int amount) =>
            Observable.Create<int>((observer, cancellationToken) => Run(() =>
            {
                static IEnumerable<int> Generate(int amount) =>
                    from i in Enumerable.Range(2, amount - 1).AsParallel()
                    where Enumerable.Range(1, (int)Math.Sqrt(i)).All(j => j == 1 || i % j != 0)
                    select i;
                
                foreach (var prime in Generate(amount))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    observer.OnNext(prime);
                }

                observer.OnCompleted();
            }));


    }
}