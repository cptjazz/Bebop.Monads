// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using BenchmarkDotNet.Attributes;
using System;

namespace Bebop.Monads
{
    [SimpleJob]
    [RPlotExporter, RankColumn]
    [DisassemblyDiagnoser(printSource: true)]
    public class MaybeBenchmarks
    {
        [Params(1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
        }
        
        [Benchmark]
        public Maybe<int> GenericValueTypeFrom() => Maybe.From(123);

        [Benchmark]
        public Maybe<int> GenericValueTypeNothing() => Maybe.Nothing<int>();

        [Benchmark]
        public Maybe<string> GenericReferenceTypeFrom() => Maybe.From("asdf");

        [Benchmark]
        public Maybe<string> GenericReferenceTypeNothing() => Maybe.Nothing<string>();
        
        [Benchmark]
        public IMaybe NonGenericValueTypeFrom() => Maybe.From(typeof(int), 123);

        [Benchmark]
        public IMaybe NonGenericValueTypeNothing() => Maybe.Nothing(typeof(int));

        [Benchmark]
        public IMaybe NonGenericReferenceTypeFrom() => Maybe.From(typeof(string), "asdf");

        [Benchmark]
        public IMaybe NonGenericReferenceTypeNothing() => Maybe.Nothing(typeof(string));
        /*
        [Benchmark]
        public IMaybe NonGenericCastableValueTypeFrom() => Maybe.Castable.From(typeof(int), 123);

        [Benchmark]
        public IMaybe NonGenericCastableValueTypeNothing() => Maybe.Castable.Nothing(typeof(int));

        [Benchmark]
        public IMaybe NonGenericCastableReferenceTypeFrom() => Maybe.Castable.From(typeof(string), "asdf");

        [Benchmark]
        public IMaybe NonGenericCastableReferenceTypeNothing() => Maybe.Castable.Nothing(typeof(string));
    */}
}
