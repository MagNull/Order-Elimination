using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    //https://dotnetfiddle.net - Unity online compiler

    public readonly struct CellRangeBorders
    {
        public readonly int xMin;
        public readonly int yMin;
        public readonly int xMax;
        public readonly int yMax;

        public CellRangeBorders(Vector2Int start, Vector2Int end)
        {
            xMin = Math.Min(start.x, end.x);
            yMin = Math.Min(start.y, end.y);
            xMax = Math.Max(start.x, end.x);
            yMax = Math.Max(start.y, end.y);
        }

        public CellRangeBorders(int xStart, int yStart, int xEnd, int yEnd)
        {
            xMin = Math.Min(xStart, xEnd);
            yMin = Math.Min(yStart, yEnd);
            xMax = Math.Max(xStart, xEnd);
            yMax = Math.Max(yStart, yEnd);
        }

        public bool Contains(int x, int y) => x >= xMin && y >= yMin && x <= xMax && y <= yMax;

        public bool Contains(Vector2Int point) => Contains(point.x, point.y);

        public Vector2Int[,] GetContainingCellPositions()
        {
            var result = new Vector2Int[xMax - xMin + 1, yMax - yMin + 1];
            for (var y = yMin; y <= yMax; y++)
            {
                for (var x = xMin; x <= xMax; x++)
                {
                    result[x, y] = new Vector2Int(x, y);
                }
            }
            return result;
        }

        public IEnumerable<Vector2Int> EnumerateCellPositions()
        {
            for (var y = yMin; y <= yMax; y++)
            {
                for (var x = xMin; x <= xMax; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }
    }
}
