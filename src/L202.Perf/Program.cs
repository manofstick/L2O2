using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace L202.Perf
{
    //enum Library
    //{
    //    L2O2,
    //    Linq
    //}

    enum DataStructure
    {
        Array,
        List,
        Range,
        Enumerable
    }

    enum Function
    {
        Foreach,
        All
    }


    class Program
    {
        static void Main(string[] args)
        {
            //var library = Library.L2O2;
            var dataStructure = DataStructure.Enumerable;
            var function = Function.Foreach;
            (
                string __FUNCTIONS__,
                Func<IEnumerable<int>, Func<int, int>, IEnumerable<int>> __SELECT__,
                Func<IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> __SELECTI__,
                Func<IEnumerable<int>, Func<int, bool>, bool> __ALL__,
                Func<IEnumerable<int>, Func<int, bool>, IEnumerable<int>> __WHERE__,
                Func<IEnumerable<int>, Func<int, int,bool>, IEnumerable<int>> __WHEREI__
            ) =
#if false
            (
                "L2O2",
                L2O2.Enumerable.Select,
                L2O2.Enumerable.Select,
                L2O2.Enumerable.All,
                L2O2.Enumerable.Where,
                L2O2.Enumerable.Where
            );
#else
            (
                "Linq",
                System.Linq.Enumerable.Select,
                System.Linq.Enumerable.Select,
                System.Linq.Enumerable.All,
                System.Linq.Enumerable.Where,
                System.Linq.Enumerable.Where
            );
#endif
            System.Console.WriteLine($"{__FUNCTIONS__} {dataStructure} {function} ({DateTime.Now})\n--");

            for (var orderIdx = 1; orderIdx < 10; ++orderIdx)
            {
                var elements = (int)Math.Pow(2, orderIdx) - 1;
                var iterations = 10000000 / (elements + 1);

                Console.Write($"{elements}:");

                IEnumerable<int> source = System.Linq.Enumerable.Range(0, elements);
                if (dataStructure == DataStructure.Array)
                    source = System.Linq.Enumerable.ToArray(source);
                else if (dataStructure == DataStructure.List)
                    source = System.Linq.Enumerable.ToList(source);
                else if (dataStructure == DataStructure.Enumerable)
                    source = GetEnumerable(elements);
                else if (dataStructure == DataStructure.Range)
                { }
                else
                    throw new Exception("bad DataStructure");

                var innerIterations = 5;
                var totalTime = 0L;
                var checksum = 0;
                for (var count = 0; count < innerIterations; ++count)
                {
                    var sw = Stopwatch.StartNew();

                    for (var i = 0; i < iterations; ++i)
                    {
                        var data = source;

                        data = __SELECT__(data, x => x + 1);
                        data = __WHEREI__(data, (x,ii) => x + ii > 5);
                        //data = __WHERE__(data, x => x != 42);
                        //data = __SELECT__(data, x => x + 1);
                        //data = __SELECT__(data, x => x + 1);
                        //data = __SELECT__(data, x => x + 1);
                        //data = __SELECT__(data, x => x + 1);
                        //data = __SELECT__(data, x => x + 1);
                        //data = __SELECT__(data, x => x + 1);
                        //data = __SELECT__(data, x => x + 1);

                        switch (function)
                        {
                            case Function.Foreach:
                                foreach (var item in data)
                                    checksum += item;
                                break;

                            case Function.All:
                                if (__ALL__(data, x => x >= 0))
                                    checksum += 1;
                                break;

                            default: throw new Exception("bad Function");
                        }
                    }
                    var time = sw.ElapsedMilliseconds;
                    totalTime += time;

                    System.Console.Write($"{time},");
                }
                System.Console.WriteLine($"{checksum}\t\t{totalTime / innerIterations}");
            }
        }

        private static IEnumerable<int> GetEnumerable(int elements)
        {
            for (var i = 0; i < elements; ++i)
                yield return i;
        }
    }
}
