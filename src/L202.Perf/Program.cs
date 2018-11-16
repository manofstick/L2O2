using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace L202.Perf
{
    class Program
    {
        static void Main(string[] args)
        {
            (
                string __FUNCTIONS__,
                Func<IEnumerable<int>, Func<int, int>, IEnumerable<int>> __SELECT__,
                Func<IEnumerable<int>, Func<int, bool>, bool> __ALL__
            ) =
#if false
            (
                "L202",
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

            for (var orderIdx = 0; orderIdx < 10; ++orderIdx)
            {
                var elements = (int)Math.Pow(2, orderIdx);
                var iterations = 10000000 / elements;

                System.Console.Write($"{__FUNCTIONS__}:{elements}:");

                IEnumerable<int> data = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Range(0, elements));

                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);
                data = __SELECT__(data, x => x + 1);

                var checksum = 0;
                for (var count = 0; count < 5; ++count)
                {
                    var sw = Stopwatch.StartNew();

                    for (var i = 0; i < iterations; ++i)
                    {
                        if (__ALL__(data, x => x >= 0))
                            checksum += 1;
                    }

                    System.Console.Write($"{sw.ElapsedMilliseconds},");
                }
                System.Console.WriteLine(checksum);
            }
        }
    }
}
