using System;
using System.Diagnostics;
using System.Linq;
using gt.Collections.Ordered;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class OrderedListPerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public OrderedListPerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ManyRandomInsertions()
        {
            var random = new Random();
            var orders = Enumerable.Range(0, 200000).Select(i=>random.Next());
            var list = new OrderedList<int>(n=>n);
            var sw = Stopwatch.StartNew();
            sw.Start();
            foreach (var order in orders)
                list.Add(order);
            sw.Stop();

            _output.WriteLine(sw.Elapsed.ToString());
            list.AssertIsSorted(o => o);
        }
    }
}