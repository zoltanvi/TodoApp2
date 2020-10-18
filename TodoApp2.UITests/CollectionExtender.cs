using System.Collections.Generic;

namespace TodoApp2.UITests.Helpers
{
    public static class CollectionExtender
    {
        /// <summary>Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.</summary>
        /// <param name="currentCollection">The collection to expand.</param>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="ICollection{T}" />.
        /// The collection itself cannot be <see langword="null" />, but it can contain elements that are <see langword="null" />,
        /// if type <paramref name="T" /> is a reference type.</param>
        /// <exception cref="System.ArgumentNullException">
        /// if <paramref name="collection" /> is <see langword="null" />.</exception>
        public static void AddRange<T>(this ICollection<T> currentCollection, IEnumerable<T> collection)
        {
            foreach (T obj in collection)
            {
                currentCollection.Add(obj);
            }
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            T obj = list[oldIndex];
            list.Remove(obj);
            list.Insert(newIndex, obj);
        }
    }
}