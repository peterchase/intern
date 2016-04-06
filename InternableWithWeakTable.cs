using System;
using System.Runtime.CompilerServices;

namespace Intern
{
    /// <summary>
    /// This class is interned, but should avoid uncontrolled growth of the intern table, by some fancy shenanigans
    /// with weak references. However, this implementation requires the internable class to have value-based equality
    /// and hashing semantics, which is a shame because it means we cannot benefit from faster reference-based versions.
    /// </summary>
    public class InternableWithWeakTable : IEquatable<InternableWithWeakTable>
    {
        private static readonly ConditionalWeakTable<InternableWithWeakTable, WeakReference<InternableWithWeakTable>> Interns = new ConditionalWeakTable<InternableWithWeakTable, WeakReference<InternableWithWeakTable>>();

        private readonly int number;
        private readonly string text;

        private InternableWithWeakTable(int number, string text)
        {
            this.number = number;
            this.text = text;
        }

        public int Number { get { return number; } }

        public string Text { get { return text; } }

        public static InternableWithWeakTable Intern(int number, string text)
        {
            var possiblyNonCanonicalInstance = new InternableWithWeakTable(number, text);
            for (;;)
            {
                WeakReference<InternableWithWeakTable> weakReference = Interns.GetValue(possiblyNonCanonicalInstance, c => new WeakReference<InternableWithWeakTable>(c));

                InternableWithWeakTable canonicalInstance;
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

        public bool Equals(InternableWithWeakTable other)
        {
            if (other == null)
            {
                return false;
            }

            return number == other.number && Equals(text, other.text);
        }

        public override int GetHashCode()
        {
            return number ^ (text == null ? 0 : text.GetHashCode());
        }
    }
}
