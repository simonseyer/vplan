using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            foreach (T item in newItems)
            {
                collection.Add(item);
            }
        }

        public static void UpdateOrInsert<T>(this IList<T> collection, T newItem) where T: IComparable
        {
            // TODO: use a binary search
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                var comparison = item.CompareTo(newItem);
                if (comparison == 0)
                {
                    collection[i] = newItem;
                    return;
                }
                if (comparison > 0)
                {
                    collection.Insert(i, newItem);
                    return;
                }
            }
            collection.Add(newItem);
        }

        public static void UpdateOrInsertRange<T>(this IList<T> collection, IEnumerable<T> newItems) where T : IComparable
        {
            foreach (T item in newItems)
            {
                collection.UpdateOrInsert(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }
    }
}
