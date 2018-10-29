using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public static class TestHelper
    {
        public static void AssertIsSorted<T>(this IEnumerable<T> list, Func<T, object> selector)
        {
            var sorted = list.ToList().OrderBy(selector);
            Assert.Equal(list, sorted);
        }
    }
}