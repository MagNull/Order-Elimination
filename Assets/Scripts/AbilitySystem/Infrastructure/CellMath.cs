using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public CellRangeBorders(RectInt rect)
        {
            xMin = rect.xMin;
            yMin = rect.yMin;
            xMax = rect.xMax;
            yMax = rect.yMax;
        }

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

        public bool Contains(int x, int y) => (x >= xMin || y >= yMin || x <= xMax || y <= yMax);

        public bool Contains(Vector2Int point) => Contains(point.x, point.y);

        public Vector2Int[,] GetContainingCells()
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

    public readonly struct CellIntersection
    {
        public readonly Vector2Int CellPosition;
        public readonly double IntersectionAngle;
        public readonly double SmallestPartSquare;

        public CellIntersection(Vector2Int cellPosition, double intersectionAngle, double smallestPartSquare)
        {
            CellPosition = cellPosition;
            IntersectionAngle = intersectionAngle;
            SmallestPartSquare = smallestPartSquare;
        }
    }

    public class PointRelativePattern
    {
        public IEnumerable<Vector2Int> RelativePositions => _relativePositions;
        private readonly HashSet<Vector2Int> _relativePositions = new HashSet<Vector2Int>();

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            return _relativePositions.Select(v => originPoint + v).ToArray();
        }

        public bool AddRelativePosition(Vector2Int offset) => _relativePositions.Add(offset);

        public bool RemoveRelativePosition(Vector2Int offset) => _relativePositions.Remove(offset);
    }

    public static class CellMath
    {
        public static CellIntersection[] GetIntersectionBetween(Vector2Int startPoint, Vector2Int endPoint, bool includeEnd = false, bool includeStart = false)
        {
            var radiusVector = endPoint - startPoint;
            var acos = Math.Acos(radiusVector.x / Math.Sqrt(radiusVector.x * radiusVector.x + radiusVector.y * radiusVector.y));
            var angle = radiusVector.y < 0 ? -acos + 2 * Math.PI : acos;
            var limitingRect = new CellRangeBorders(startPoint, radiusVector);
            var intersections = new List<CellIntersection>();//GetRayIntersection(startPoint, angle, limitingRect).ToList();
            foreach (var intersection in GetRayIntersection(startPoint, angle))
            {
                if (!limitingRect.Contains(intersection.CellPosition))
                    break;
                intersections.Add(intersection);
            }
            if (!includeStart)
                intersections.RemoveAt(0);
            if (!includeEnd)
                intersections.RemoveAt(intersections.Count);
            return intersections.ToArray();
        }

        //Из-за ущербной погрешности лучше производить расчёты не на основе угла, а на векторах, проверяя x1=x2 || y1=y2 || 
        //Убрать limitingRect. Сделать коллекцию бесконечным IEnumerable<CellIntersection>. Потом проверять battleMap.rect.Contains()
        //public static CellIntersection[] ПолучитьПересечениеЛучом(начТочка, направление)
        //начало включено
        public static IEnumerable<CellIntersection> GetRayIntersection(Vector2Int startPoint, double angle)
        {
            //angle %= 2 * Math.PI;
            //if (angle < 0) angle += 2 * Math.PI;
            //var intersections = new List<CellIntersection>();
            //if (Math.Round(angle % (Math.PI / 4), 4) == 0)
            //{
            //    //Угол кратен 45 (Pi/4)
            //    var point = startPoint;
            //    while (limitingRect.Contains(point))
            //    {
            //        var cos = Math.Round(Math.Cos(angle), 6);
            //        var sin = Math.Round(Math.Sin(angle), 6);
            //        var nextX = point.x + Math.Sign(cos) * 1;
            //        var nextY = point.y + Math.Sign(sin) * 1;
            //        point = new Vector2Int(nextX, nextY);
            //        if (limitingRect.Contains(point))
            //            intersections.Add(new CellIntersection(point, (float)(angle % (2 * Mathf.PI)), 0.5f));
            //    }
            //    return intersections.ToArray();
            //}

            //var k = (float)Math.Round(Math.Tan(angle), 6);
            //var b = startPoint.y - k * startPoint.x;
            //Func<float, float> y = x => k * x;
            //Func<float, float> x = y => k * y;
            //Дальше платно
            throw new NotImplementedException();
        }
    }
}
