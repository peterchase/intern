using Intern;
using NUnit.Framework;

namespace InternTests
{
    [TestFixture]
    public class InternableTests
    {
        [Test]
        public void Intern_ShouldReturnSameObject_WhenCalledWithSameParameters()
        {
            var o1 = Internable.Intern(123, "foo");
            var o2 = Internable.Intern(123, "foo");

            Assert.That(o2, Is.SameAs(o1));
            Assert.That(o1.Number, Is.EqualTo(123));
            Assert.That(o1.Text, Is.EqualTo("foo"));
        }

        [Test]
        public void Intern_ShouldReturnCorrectObjects_WhenCalledWithDifferentParameters()
        {
            var o1 = Internable.Intern(123, "foo");
            var o2 = Internable.Intern(234, "bar");

            Assert.That(o1.Number, Is.EqualTo(123));
            Assert.That(o1.Text, Is.EqualTo("foo"));

            Assert.That(o2.Number, Is.EqualTo(234));
            Assert.That(o2.Text, Is.EqualTo("bar"));
        }
    }
}
