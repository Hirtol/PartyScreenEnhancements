using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.Patches
{
    public static class MBBindingListExtensions
    {

        /// <summary>
        /// Sort the <code>MBBindingList</code> using a stable sort, instead of the standard unstable sort (which screws up hero orderings once in a while).
        /// Uses 2 pieces of reflection to first, get the internal <code>List</code> of the MBBindingList,
        /// and afterward get the internal <code>_items</code> array of the List.
        ///
        /// This is not optimal and VERY fragile, Enumerable would be preferable, but for some reason it seems to freeze the game
        /// whenever we try to .. enumerate the enumeration.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comparer"></param>
        public static void StableSort<T>(this MBBindingList<T> list, IComparer<T> comparer)
        {
            if (list.IsOrdered(comparer))
                return;

            var traverse = Traverse.Create(list);
            var internalList = traverse.Field<List<T>>("_list").Value;
            internalList.StableSort(comparer);
            traverse.Method("FireListChanged", new[] { typeof(ListChangedType), typeof(int) },
                new object[] { ListChangedType.Sorted, -1 }).GetValue();
        }

        // Below two extension methods were taken from StackOverflow: https://stackoverflow.com/questions/148074/is-the-sorting-algorithm-used-by-nets-array-sort-method-a-stable-algorithm
        // with some minor modifications.
        public static void StableSort<T>(this List<T> values, IComparer<T> comparer)
        {
            var keys = new KeyValuePair<int, T>[values.Count];
            for (var i = 0; i < values.Count; i++)
                keys[i] = new KeyValuePair<int, T>(i, values[i]);
            // Bad.
            var internalListArray = Traverse.Create(values).Field<T[]>("_items").Value;
            Array.Sort(keys, internalListArray, new StabilizingComparer<T>(comparer));
        }

        private sealed class StabilizingComparer<T> : IComparer<KeyValuePair<int, T>>
        {
            private readonly IComparer<T> _comparison;

            public StabilizingComparer(IComparer<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(KeyValuePair<int, T> x,
                KeyValuePair<int, T> y)
            {
                var result = _comparison.Compare(x.Value, y.Value);
                return result != 0 ? result : x.Key.CompareTo(y.Key);
            }
        }
    }
}