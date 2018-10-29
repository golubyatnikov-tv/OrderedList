using System.Collections.Generic;
using gt.Collections.Ordered.Helpers;
using Xunit;

namespace Tests
{
    public class BisectTests
    {
        [Fact]
        public void Test1()
        {            
            var res = BinarySearchHelper.BinarySearch(new[] {1, 2, 4, 5}, 3, null);
            Assert.Equal(~2, res);
        }

        [Fact]
        public void Test2()
        {
            var res = BinarySearchHelper.BinarySearch(new[] { 1,2,3 }, 2, null);            
            Assert.InRange(res,1,2);
        }

        [Fact]
        public void Test3()
        {
            var res = BinarySearchHelper.BinarySearch(new[] { 1, 2, 2, 3 }, 1, null);
            Assert.InRange(res, 0, 1);            
        }
    }
}