﻿using System.Collections.Generic;

namespace Exanite.Core.Utilities
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> AddToStart<T>(this IEnumerable<T> collection, T toStart)
        {
            yield return toStart;

            foreach (var item in collection)
            {
                yield return item;
            }
        }
        
        public static IEnumerable<T> AddToStart<T>(this IEnumerable<T> collection, IEnumerable<T> toStart)
        {
            foreach (var element in toStart)
            {
                yield return element;
            }

            foreach (var element in collection)
            {
                yield return element;
            }
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> collection, T toEnd)
        {
            foreach (var element in collection)
            {
                yield return element;
            }

            yield return toEnd;
        }
        
        public static IEnumerable<T> Add<T>(this IEnumerable<T> collection, IEnumerable<T> toEnd)
        {
            foreach (var item in collection)
            {
                yield return item;
            }

            foreach (var element in toEnd)
            {
                yield return element;
            }
        }
    }
}