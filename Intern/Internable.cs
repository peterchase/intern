using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Intern
{
    /// <summary>
    /// This class is internable and has private constructor, so client code cannot possibly obtain a non-canonical instance.
    /// It uses a custom comparer internally, so that this class can keep the default, fast reference-based equality and hashing
    /// semantics. However, there is no protection against the intern table becoming stuffed with dead instances over time.
    /// </summary>
    public class Internable
    {
        private static readonly ConcurrentDictionary<Internable, Internable> Interns = new ConcurrentDictionary<Internable, Internable>(new ValueBasedComparer());

        private readonly int number;
        private readonly string text;

        private Internable(int number, string text)
        {
            this.number = number;
            this.text = text;
        }

        public int Number { get { return number; } }

        public string Text { get { return text; } }

        public static Internable Intern(int number, string text)
        {
            var possiblyNonCanonicalInstance = new Internable(number, text);
            return Interns.GetOrAdd(possiblyNonCanonicalInstance, possiblyNonCanonicalInstance);
        }

        private class ValueBasedComparer : IEqualityComparer<Internable>
        {
            public bool Equals(Internable x, Internable y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.number == y.number && Equals(x.text, y.text);
            }

            public int GetHashCode(Internable obj)
            {
                return obj.number ^ (obj.text == null ? 0 : obj.text.GetHashCode());
            }
        }
    }
}
