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
            var dataStructure = DataStructure.Array;
            var function = Function.All;
            (
                string __FUNCTIONS__,
                Func<IEnumerable<int>, Func<int, int>, IEnumerable<int>> __SELECT__,
                Func<IEnumerable<int>, Func<int, bool>, bool> __ALL__
            ) =
#if false
            (
                "L2O2",
                L2O2.Enumerable.Select,
                L2O2.Enumerable.All
            );
#else
            (
                "Linq",
                System.Linq.Enumerable.Select,
                System.Linq.Enumerable.All
            );
#endif
            System.Console.WriteLine($"{__FUNCTIONS__} {dataStructure} {function} ({DateTime.Now})\n--");

            for (var orderIdx = 0; orderIdx < 10; ++orderIdx)
            {
                var elements = (int)Math.Pow(2, orderIdx)-1;
                var iterations = 10000000 / (elements+1);

                Console.Write($"{elements}:");

                IEnumerable<int> data = System.Linq.Enumerable.Range(0, elements);
                if (dataStructure == DataStructure.Array)
                    data = System.Linq.Enumerable.ToArray(data);
                else if (dataStructure == DataStructure.List)
                    data = System.Linq.Enumerable.ToList(data);
                else if (dataStructure == DataStructure.Enumerable)
                { }
                else
                    throw new Exception("bad DataStructure");

                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);

                var innerIterations = 5;
                var totalTime = 0L;
                var checksum = 0;
                for (var count = 0; count < innerIterations; ++count)
                {
                    var sw = Stopwatch.StartNew();

                    for (var i = 0; i < iterations; ++i)
                    {
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
                System.Console.WriteLine($"{checksum}\t\t{totalTime/innerIterations}");
            }
        }
    }
}
