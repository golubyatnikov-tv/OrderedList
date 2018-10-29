using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gt.Collections.Ordered;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace Tests
{
    public class OrderedCollectionTests
    {
        [Fact]
        public void AddShouldSort()
        {
            var list = OrderedCollection<int>.Create(n => n);            
            list.Add(2);
            list.Add(-1);
            list.Add(3);
            list.Add(0);

            Assert.Equal(new[] { -1, 0, 2, 3 }, list);
            list.AssertIsSorted(o => o);
        }

        [Fact]
        public void AddItemWithSameOrderValueShouldBeAddedToEndOfGroup()
        {
            var list = OrderedCollection<TestObj>.Create(n => n.Order);
            list.Add(new TestObj(2));
            list.Add(new TestObj(2));
            list.Add(new TestObj(3));
            var thirdObject = new TestObj(2);
            list.Add(thirdObject);

            Assert.Equal(thirdObject, list.ElementAt(2));
            list.AssertIsSorted(o => o.Order);
        }

        [Fact]
        public void ItemShouldBeMovedOnOrderPropertyChanged()
        {
            var list = OrderedCollection<TestNotifyObj>.Create(n => n.Order);
            var o1 = new TestNotifyObj(2);
            var o2 = new TestNotifyObj(3);
            var o0_1 = new TestNotifyObj(1);
            list.Add(o1);
            list.Add(o2);
            var testObj = o0_1;
            list.Add(testObj);

            Assert.Equal(new[] { o0_1, o1, o2 }, list);

            testObj.Order = 2;            
            Assert.Equal(new[] { o1, o0_1, o2 }, list);

            list.AssertIsSorted(o => o.Order);
        }

        [Fact]
        public void RandomTest()
        {
            var list = OrderedCollection<TestNotifyObj>.Create(n => n.Order);
            var random = new Random();
            var indices = Enumerable.Range(1, 10000).ToList();
            var items = indices.Select(i => new TestNotifyObj(i % 100)).ToList();
            foreach (var item in items)
                list.Add(item);

            list.AssertIsSorted(i=>i.Order);

            foreach (var item in items)
                item.Order = random.Next(0, 99);
            list.AssertIsSorted(i=>i.Order);
        }
    }
}
