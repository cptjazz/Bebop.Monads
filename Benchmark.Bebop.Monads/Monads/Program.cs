// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using BenchmarkDotNet.Running;

namespace Bebop.Monads
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MaybeBenchmarks>();
        }
    }
}
