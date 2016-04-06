using System;
using System.Runtime.CompilerServices;

namespace Intern
{
    /// <summary>
    /// This class is interned, but should avoid uncontrolled growth of the intern table, by some fancy shenanigans
    /// with weak references. By using a wrapper key class, with value-based equality and hashing, this class can
    /// keep the fast, default reference-based equality and hashing.
    /// </summary>
    public class InternableWithWeakTableAndValueKey
    {
        private static readonly ConditionalWeakTable<Key, WeakReference<InternableWithWeakTableAndValueKey>> Interns = new ConditionalWeakTable<Key, WeakReference<InternableWithWeakTableAndValueKey>>();

        private readonly int number;
        private readonly string text;

        private InternableWithWeakTableAndValueKey(int number, string text)
        {
            this.number = number;
            this.text = text;
        }

        public int Number { get { return number; } }

        public string Text { get { return text; } }

        public static InternableWithWeakTableAndValueKey Intern(int number, string text)
        {
            var possiblyNonCanonicalInstance = new InternableWithWeakTableAndValueKey(number, text);
            var key = new Key(possiblyNonCanonicalInstance);
            for (;;)
            {
                WeakReference<InternableWithWeakTableAndValueKey> weakReference = Interns.GetValue(key, c => new WeakReference<InternableWithWeakTableAndValueKey>(c.Instance));

                InternableWithWeakTableAndValueKey canonicalInstance;
                if (weakReference.TryGetTarget(out canonicalInstance))
                {
                    // Either we made a new weak reference just now, or an existing weak reference was still live
                    return canonicalInstance;
                }

                // Between getting the weak reference out of the weak table and dereferencing the weak reference, the object might have
                // ceased to be referenced, due to actions in other threads or by GC. However, if that is the case, the weak table will
                // no longer have an entry, so if we go around the loop again, we should create a new entry.
            }
        }

        private class Key : IEquatable<Key>
        {
            private readonly InternableWithWeakTableAndValueKey instance;

            public Key(InternableWithWeakTableAndValueKey instance)
            {
                this.instance = instance;
            }

            public InternableWithWeakTableAndValueKey Instance { get { return instance; } }


            public bool Equals(Key other)
            {
                if (other == null || other.instance == null)
                {
                    return false;
                }

                return instance.number == other.instance.number && Equals(instance.text, other.instance.text);
            }

            public override int GetHashCode()
            {
                return instance.number ^ (instance.text == null ? 0 : instance.text.GetHashCode());
            }
        }
    }
}
