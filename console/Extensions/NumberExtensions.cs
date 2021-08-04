using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace console.Extensions
{
    public static class NumberExtensions
    {
        public static async Task<bool> IsPrimeNumber(this int number) =>
            await  Task.Run(() => NumberGenerator.GeneratePrimes(10).ToEnumerable().Contains(number));
    }
}