using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CafeSystem.Utils
{
    [DebuggerStepThrough]
    public static class CollectionExts
    {
#if NETSTANDARD2_0
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer
 = null)
        => source.GroupBy(keySelector, comparer).Select(x => x.First());
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T>? ForEach<T>(this IEnumerable<T>? source, Action<T> action)
        {
            if (source is not null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }

            return source;
        }


        /// <summary> Add the object top the end of IEnumerable </summary>
        /// <param name="element">object to append</param>
        /// <returns>updated IEnumerable</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        {
            foreach (var item in source)
            {
                yield return item;
            }

            yield return element;
        }


        /// <summary> Check if all elements in IEnumerable are equals </summary>
        /// <exception cref="System.ArgumentNullException">enumerable is null</exception>
        /// <returns>true if they are equals, otherwise - false</returns>
        [DebuggerStepThrough]
        public static bool AreAllSame<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            using (var enumerator = enumerable.GetEnumerator())
            {
                var toCompare = default(T);
                if (enumerator.MoveNext())
                {
                    toCompare = enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    if (toCompare != null && !toCompare.Equals(enumerator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Extracts the elements of an IEnumerable<string> to string.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns>concatenated string</returns>
        [DebuggerStepThrough]
        public static string ExtractAsString(this IEnumerable<string> enumerable)
        {
            var sb = new StringBuilder();

            foreach (var s in enumerable)
            {
                sb.AppendLine(s);
            }

            return sb.ToString();
        }

        public static async Task<IEnumerable<T>> ForEachAsync<T>(
            this IEnumerable<T>? source,
            Func<T, Task> action,
            CancellationToken cancellationToken = default)
        {
            if (source is not null)
            {
                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await action.Invoke(item).ConfigureAwait(false);
                }
            }

            return source;
        }

        public static async Task<IEnumerable<TResult>?> SelectAsync<TSource, TResult>(
            this IEnumerable<TSource>? source,
            Func<TSource, Task<TResult>> asyncSelector,
            CancellationToken cancellationToken = default)
        {
            if (source is not null)
            {
                var result = new List<TResult>();
                foreach (var item in source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    result.Add(await asyncSelector(item).ConfigureAwait(false));
                }

                return result;
            }

            return null;
        }

#if NET6
    public static async ValueTask<List<TSource>> ToListAsync<TSource>(
        this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
    {
        var list = new List<TSource>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            list.Add(item);
        }

        return list;
    }

#endif

        public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                var element = collection.ElementAt(i);
                if (predicate(element))
                {
                    collection.Remove(element);
                }
            }
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
            => source.Any();

        public static bool IsEmpty<T>(this IEnumerable<T> source)
            => !source.IsNotEmpty();

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
            => !source?.Any() ?? true;

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
            => !source.IsNullOrEmpty();

        public static int GetCount<T>(this IEnumerable<T>? source, Func<T, bool>? predicate = null)
            => source?.Count(predicate!) ?? 0;

        public static long GetLongCount<T>(this IEnumerable<T>? source, Func<T, bool>? predicate = null)
            => source?.LongCount(predicate!) ?? 0;


        /// <summary>
        /// Concatenates the elements of an IEnumerable, using the specified separator between each element
        /// </summary>
        /// <param name="separator">
        /// The string to use as a separator. separator is included in the returned string
        /// only if values has more than one element/
        /// </param>
        /// <exception cref="System.ArgumentNullException">if values is null</exception>
        /// <returns>
        /// A string that consists of the elements of values delimited by the separator string.
        /// If values is an empty IEnumerable, the method returns String.Empty.
        /// </returns>
        [DebuggerStepThrough]
        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
            {
                return string.Empty;
            }

            return string.Join(separator, source.Select(e => e.ToString()).ToArray());
        }
        
        /// <summary>
        /// Finds the index of the first item matching an expression in a collection.
        /// </summary>
        ///<param name="collection">The collection to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this ICollection<T> collection, Predicate<T> predicate)
        {
            return FindIndexInternal(collection, predicate);
        }
        
        ///<summary>
        /// Finds the index of the first occurrence of an item in a collection.
        /// </summary>
        ///<param name="collection">The collection to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this ICollection<T> collection, T item)
        {
            return collection.FindIndex(x => EqualityComparer<T>.Default.Equals(item, x));
        }

        /// <summary>
        /// Add range of items to observable collection.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="collection">observable collection</param>
        /// <param name="items">item to add to the collection</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
        
        #region private methods

        /// <summary>
        /// Finds the index of the first item matching an expression in a collection.
        /// </summary>
        ///<param name="collection">The collection to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        private static int FindIndexInternal<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            var result = 0;
            foreach (var item in collection)
            {
                if (predicate.Invoke(item)) return result;
                ++result;
            }

            return -1;
        }
        

        #endregion
    }
}