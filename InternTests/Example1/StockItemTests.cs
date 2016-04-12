using Intern.Example1;
using NUnit.Framework;

namespace InternTests.Example1
{
    [TestFixture]
    public class StockItemTests
    {
        [Test]
        public void Intern_ShouldReturnSameObject_WhenCalledWithSameParameters()
        {
            var o1 = StockItem.Intern(new StockItem(123, "foo"));
            var o2 = StockItem.Intern(new StockItem(123, "foo"));

            Assert.That(o2, Is.SameAs(o1));
            Assert.That(o1.StockCode, Is.EqualTo(123));
            Assert.That(o1.Description, Is.EqualTo("foo"));
        }

        [Test]
        public void Intern_ShouldReturnCorrectObjects_WhenCalledWithDifferentParameters()
        {
            var o1 = StockItem.Intern(new StockItem(123, "foo"));
            var o2 = StockItem.Intern(new StockItem(234, "bar"));

            Assert.That(o1.StockCode, Is.EqualTo(123));
            Assert.That(o1.Description, Is.EqualTo("foo"));

            Assert.That(o2.StockCode, Is.EqualTo(234));
            Assert.That(o2.Description, Is.EqualTo("bar"));
        }
    }
}
