using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Intern
{
    /// <summary>
    /// TODO
    /// </summary>
    public class InternableWithWeakRefs
    {
        private static readonly ConcurrentDictionary<WeakReference<InternableWithWeakRefs>, WeakReference<InternableWithWeakRefs>> Interns = new ConcurrentDictionary<WeakReference<InternableWithWeakRefs>, WeakReference<InternableWithWeakRefs>>(new ValueBasedComparer());

        private readonly int number;
        private readonly string text;

        private InternableWithWeakRefs(int number, string text)
        {
            this.number = number;
            this.text = text;
        }

        public int Number { get { return number; } }

        public string Text { get { return text; } }

        public static InternableWithWeakRefs Intern(int number, string text)
        {
            var possiblyNonCanonicalInstance = new InternableWithWeakRefs(number, text);
            var newWeakReference = new WeakReference<InternableWithWeakRefs>(possiblyNonCanonicalInstance);

            for (;;)
            {
                var canonicalWeakReference = Interns.GetOrAdd(newWeakReference, newWeakReference);
                InternableWithWeakRefs canonicalInstance;
                if (!canonicalWeakReference.TryGetTarget(out canonicalInstance))
                {
                    // Race condition?

                    Interns[newWeakReference] = newWeakReference;
                    canonicalInstance = possiblyNonCanonicalInstance;
                }

                return canonicalInstance;

            }
        }

        private class ValueBasedComparer : IEqualityComparer<WeakReference<InternableWithWeakRefs>>
        {
            public bool Equals(WeakReference<InternableWithWeakRefs> x, WeakReference<InternableWithWeakRefs> y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                InternableWithWeakRefs xTarget, yTarget;
                bool xLive = x.TryGetTarget(out xTarget);
                bool yLive = y.TryGetTarget(out yTarget);
                if (!(xLive && yLive))
                {
                    return false;
                }

                return xTarget.number == yTarget.number && Equals(xTarget.text, yTarget.text);
            }

            public int GetHashCode(WeakReference<InternableWithWeakRefs> obj)
            {
                InternableWithWeakRefs target;
                if (obj.TryGetTarget(out target))
                {
                    return target.number ^ (target.text == null ? 0 : target.text.GetHashCode());
                }

                return 0;
            }
        }
    }
}
