using System;
using System.Threading;
using Intern.Experimental;
using NUnit.Framework;

namespace InternTests.Experimental
{
    [TestFixture]
    public class StockItemTests
    {
        [Test]
        public void Intern_ShouldReturnSameObject_WhenCalledWithSameParameters()
        {
            var o1 = StockItem.Intern(123, "foo");
            var o2 = StockItem.Intern(123, "foo");

            Assert.That(o2, Is.SameAs(o1));
            Assert.That(o1.StockCode, Is.EqualTo(123));
            Assert.That(o1.Description, Is.EqualTo("foo"));
        }

        [Test]
        public void Intern_ShouldReturnCorrectObjects_WhenCalledWithDifferentParameters()
        {
            var o1 = StockItem.Intern(123, "foo");
            var o2 = StockItem.Intern(234, "bar");

            Assert.That(o1.StockCode, Is.EqualTo(123));
            Assert.That(o1.Description, Is.EqualTo("foo"));

            Assert.That(o2.StockCode, Is.EqualTo(234));
            Assert.That(o2.Description, Is.EqualTo("bar"));
        }

        [Test]
        public void Finalise_ShouldClearInternTable_WhenNoStrongReferences()
        {
            GC.Collect(2, GCCollectionMode.Forced);

            {
                StockItem.Intern(123, "foo");
                StockItem.Intern(234, "bar");
            }

            GC.Collect(2, GCCollectionMode.Forced);

            // It only passes if this is included. Looks like even Forced GC is not really finished when the Collect() method returns.
            Thread.Sleep(100);

            Assert.That(StockItem.NumInterns, Is.EqualTo(0));
        }
    }
}
