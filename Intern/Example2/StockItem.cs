using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Intern.Example2
{
    /// <summary>
    /// This class is internable and has private constructor, so client code cannot possibly obtain a non-canonical instance.
    /// It uses a custom comparer internally, so that this class can keep the default, fast reference-based equality and hashing
    /// semantics. 
    /// <para>There is no protection against the intern table becoming stuffed with dead instances over time.</para>
    /// </summary>
    public class StockItem
    {
        private static readonly ConcurrentDictionary<StockItem, StockItem> Interns = new ConcurrentDictionary<StockItem, StockItem>(new ValueBasedComparer());

        private readonly int stockCode;
        private readonly string description;

        private StockItem(int stockCode, string description)
        {
            this.stockCode = stockCode;
            this.description = description;
        }

        public int StockCode => stockCode;

        public string Description => description;

        public static StockItem Intern(int number, string text)
        {
            var possiblyNonCanonicalInstance = new StockItem(number, text);
            return Interns.GetOrAdd(possiblyNonCanonicalInstance, possiblyNonCanonicalInstance);
        }

        private class ValueBasedComparer : IEqualityComparer<StockItem>
        {
            public bool Equals(StockItem x, StockItem y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.stockCode == y.stockCode && Equals(x.description, y.description);
            }

            public int GetHashCode(StockItem obj)
            {
                return obj.stockCode ^ (obj.description?.GetHashCode() ?? 0);
            }
        }
    }
}
