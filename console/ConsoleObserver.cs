using System;

namespace console
{
    public class ConsoleObserver<T> : IObserver<T>
    {
        private readonly string _name;

        public ConsoleObserver(string name = "")
        {
            _name = name;
        }

        public void OnNext(T value)
        {
            Console.Out.WriteLine($"{_name} - OnNext({value})");
        }

        public void OnError(Exception error)
        {
            Console.Out.WriteLine($"{_name} - OnError:");
            Console.Out.WriteLine($"\t {error.Message}");
        }

        public void OnCompleted()
        {
            Console.Out.WriteLine($"{_name} - OnCompleted()");
        }
    }
}