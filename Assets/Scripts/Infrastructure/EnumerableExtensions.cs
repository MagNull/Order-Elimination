using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<object> AsEnumerable(this ICollection collection)
        {
            foreach (var item in collection)
            {
                yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static TOut[,] Select<TIn, TOut>(this TIn[,] array, Func<TIn, TOut> selector)
        {
            var width = array.GetLength(0);
            var height = array.GetLength(1);
            var result = new TOut[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    result[x, y] = selector(array[x, y]);
                }
            }
            return result;
        }

        public static void FillIntersectionWith<T>(this T[,] arrayToFill, T[,] source)
        {
            if (arrayToFill == null || source == null)
                throw new ArgumentNullException();

            var minWidth = Math.Min(arrayToFill.GetLength(0), source.GetLength(0));
            var minHeight = Math.Min(arrayToFill.GetLength(1), source.GetLength(1));

            for (var x = 0; x < minWidth; x++)
            {
                for (var y = 0; y < minHeight; y++)
                {
                    arrayToFill[x, y] = source[x, y];
                }
            }
        }

        public static Vector2Int[,] GetIndexVectorMatrix(int width, int height)
        {
            var matrix = new Vector2Int[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    matrix[x, y] = new Vector2Int(x, y);
                }
            }
            return matrix;
        }
    }
}
