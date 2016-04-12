using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Intern.Experimental
{
    /// <summary>
    /// This tries to do interning without strong-referencing the targets. That prevents a build-up of dead items in intern table. I
    /// am however unsure whether it actually works!
    /// </summary>
    public class StockItem
    {
        private static readonly ConcurrentDictionary<WeakReference<StockItem>, WeakReference<StockItem>> Interns = new ConcurrentDictionary<WeakReference<StockItem>, WeakReference<StockItem>>(new ValueBasedComparer());

        private readonly int stockCode;
        private readonly string description;

        private StockItem(int stockCode, string description)
        {
            this.stockCode = stockCode;
            this.description = description;
        }

        public int StockCode => stockCode;

        public string Description => description;

        public static int NumInterns => Interns.Count;

        public static StockItem Intern(int number, string text)
        {
            for(;;)
            {
                var possiblyNonCanonicalInstance = new StockItem(number, text);
                var newWeakReference = new WeakReference<StockItem>(possiblyNonCanonicalInstance, true);
                WeakReference<StockItem> internedWeakReference = Interns.GetOrAdd(newWeakReference, newWeakReference);

                if (!ReferenceEquals(newWeakReference, internedWeakReference))
                {
                    // There was a different instance already in the intern table, so we do not want to execute our special finaliser
                    GC.SuppressFinalize(possiblyNonCanonicalInstance);
                }

                // Between getting the weak reference above and this point, the target could have been GC'd. If so, try again.
                StockItem itemToReturn;
                if (internedWeakReference.TryGetTarget(out itemToReturn))
                {
                    return itemToReturn;
                }
            }
        }

        ~StockItem()
        {
            var newWeakReference = new WeakReference<StockItem>(this);
            WeakReference<StockItem> dummy;
            bool removed = Interns.TryRemove(newWeakReference, out dummy);
            if (!removed)
            {
                Debugger.Log(0, null, "the value being finalised was unexpectedly not in the intern table");
                Debugger.Break();
            }
        }

        private class ValueBasedComparer : IEqualityComparer<WeakReference<StockItem>>
        {
            public bool Equals(WeakReference<StockItem> xRef, WeakReference<StockItem> yRef)
            {
                if (ReferenceEquals(xRef, yRef))
                {
                    return true;
                }

                if (xRef == null || yRef == null)
                {
                    return false;
                }

                StockItem xItem, yItem;
                if (!(xRef.TryGetTarget(out xItem) && yRef.TryGetTarget(out yItem)))
                {
                    // Nothing else we can do, but entries with GC'd targets should already have been removed by finaliser anyway
                    return false;
                }

                return xItem.stockCode == yItem.stockCode && Equals(xItem.description, yItem.description);
            }

            public int GetHashCode(WeakReference<StockItem> weakRef)
            {
                StockItem item;
                if (!weakRef.TryGetTarget(out item))
                {
                    return 0;
                }

                return item.stockCode ^ (item.description?.GetHashCode() ?? 0);
            }
        }
    }
}
