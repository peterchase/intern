using System;
using System.Collections.Concurrent;

namespace Intern.Example1PublicConstructor
{
    /// <summary>
    /// This class is internable and has public constructor, so client code could possibly obtain a non-canonical instance.
    /// It has value-based equality semantics. 
    /// <para>There is no protection against the intern table becoming stuffed with dead instances over time.</para>
    /// </summary>
    public class StockItem : IEquatable<StockItem>
    {
        // This can use the default comparer, because value-based equality semantics are implemented publically
        private static readonly ConcurrentDictionary<StockItem, StockItem> Interns = new ConcurrentDictionary<StockItem, StockItem>();

        private readonly int stockCode;
        private readonly string description;

        public StockItem(int stockCode, string description)
        {
            this.stockCode = stockCode;
            this.description = description;
        }

        public int StockCode => stockCode;

        public string Description => description;

        public static StockItem Intern(StockItem possiblyNonCanonicalInstance)
        {
            return Interns.GetOrAdd(possiblyNonCanonicalInstance, possiblyNonCanonicalInstance);
        }

        public override int GetHashCode()
        {
            return stockCode ^ (description?.GetHashCode() ?? 0);
        }

        public bool Equals(StockItem other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            return stockCode == other.stockCode && Equals(description, other.description);
        }
    }
}
