using gt.Collections.Ordered;
using gt.Collections.Ordered.Helpers;
using Xunit;

namespace Tests
{
    public class SortingHelperTests
    {
        [Fact]
        public void BisectRight_ItemNotExists_Test()
        {
            var array = new[] { 0, 1, 3 };
            var index = SortingHelper.CalculateSortedInsertIndex(array, 2, SortMethod.BisectRight);
            Assert.Equal(2, index);
        }

        [Fact]
        public void BisectRight_ItemExists_Test()
        {
            var array = new[] {0, 1, 2, 3 };
            var index = SortingHelper.CalculateSortedInsertIndex(array, 2, SortMethod.BisectRight);
            Assert.Equal(3,index);
        }

        [Fact]
        public void BisectRight_MultipleItemsExists_Test()
        {
            var array = new[] { 0, 1, 2, 2, 3 };
            var index = SortingHelper.CalculateSortedInsertIndex(array, 2, SortMethod.BisectRight);
            Assert.Equal(4, index);
        }

        [Fact]
        public void BisectLeft_ItemNotExists_Test()
        {
            var array = new[] { 0, 1, 1, 3 };
            var index = SortingHelper.CalculateSortedInsertIndex(array, 2, SortMethod.BisectLeft);
            Assert.Equal(3, index);
        }

        [Fact]
        public void BisectLeft_ItemExists_Test()
        {
            var array = new[] { 0, 1, 2, 3 };
            var index = SortingHelper.CalculateSortedInsertIndex(array, 2, SortMethod.BisectLeft);
            Assert.Equal(2, index);
        }

        [Fact]
        public void BisectLeft_MultipleItemsExists_Test()
        {
            var array = new[] { 0, 1, 1, 1, 2, 3 };
            var index = SortingHelper.CalculateSortedInsertIndex(array, 1, SortMethod.BisectLeft);
            Assert.Equal(1, index);
        }
    }
}