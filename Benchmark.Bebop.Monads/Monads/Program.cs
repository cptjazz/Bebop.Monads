// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using BenchmarkDotNet.Running;

namespace Bebop.Monads
{
    class Program
    {
        static void Main()
        {
            var _ = BenchmarkRunner.Run<MaybeBenchmarks>();
        }
    }
}
