using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maths
{
    public readonly struct CellIntersection
    {
        public readonly Vector2 CellPosition;
        public readonly double IntersectionAngle;
        public readonly double SmallestPartSquare;

        public CellIntersection(Vector2 cellPosition, double intersectionAngle, double smallestPartSquare)
        {
            CellPosition = cellPosition;
            IntersectionAngle = intersectionAngle;
            SmallestPartSquare = smallestPartSquare;
        }
    }

    public class CompereByStartPoint : IComparer<Vector2>
    {
        private Vector2 _startVector = Vector2.zero;

        public CompereByStartPoint(Vector2 startVector)
        {
            _startVector = startVector;
        }
        
        public int Compare(Vector2 x, Vector2 y)
        {
            return ((x - _startVector).magnitude).CompareTo((y - _startVector).magnitude);
        }
    }

    public static class IntersectionSolver
    {
        private static double _intersectionAngle;

        public static IEnumerable<CellIntersection> GetIntersections(Vector2 start, Vector2 end)
        {
            start += new Vector2(0.5f, 0.5f);
            end += new Vector2(0.5f, 0.5f);
            
            var points = 
                GetIntersectionPoints(start, end);

            var allIntersecions = GetDictOfIntersectionCells(points);

            foreach (var intersecion in allIntersecions)
                yield return GetCellIntersection(
                    intersecion.Key, 
                    intersecion.Value.Item1, 
                    intersecion.Value.Item2);
        }

        private static CellIntersection GetCellIntersection(Vector2 cellPos, Vector2 point1, Vector2 point2)
        {
            var interArea = 1f;
            point1 -= cellPos;
            point2 -= cellPos;
            
            var isPointOnX = (point1.x == 0 || point1.x == 1) && (point2.x == 0 || point2.x == 1);
            var isPointOnY = (point1.y == 0 || point1.y == 1) && (point2.y == 0 || point2.y == 1);

            if (isPointOnX)
            {
                interArea = GetTrapezoidArea(point1.y, point2.y);
                interArea = MathF.Min(1f - interArea, interArea);
            }
            else if (isPointOnY)
            {
                interArea = GetTrapezoidArea(point1.x, point2.x);
                interArea = MathF.Min(1f - interArea, interArea);
            }
            else
            {
                var posX = point1.x % 1 == 0 ? point1.x : point2.x;
                var posY = point1.y % 1 == 0 ? point1.y : point2.y;
                var third = new Vector2(posX, posY);
                interArea = 0.5f * (point1 - third).magnitude * (point2 - third).magnitude;
            }
            
            interArea = MathF.Min(interArea, 1f);
            
            return new CellIntersection(cellPos, _intersectionAngle, interArea);
        }

        private static Dictionary<Vector2, Tuple<Vector2, Vector2>> GetDictOfIntersectionCells(IEnumerable<Vector2> points)
        {
            var resultDict = new Dictionary<Vector2, Tuple<Vector2, Vector2>>();
            var previousPoint = points.FirstOrDefault();
            foreach (var currentPoint in points.Skip(1))
            {
                var cellX = MathF.Floor((previousPoint.x + currentPoint.x) / 2);
                var cellY = MathF.Floor((previousPoint.y + currentPoint.y) / 2);
                var cellPosition = new Vector2(cellX, cellY);
                resultDict.Add(cellPosition, new Tuple<Vector2, Vector2> (previousPoint, currentPoint));
                previousPoint = currentPoint;
            }

            return resultDict;
        }

        private static SortedSet<Vector2> GetIntersectionPoints(Vector2 start, Vector2 end)
        {
            var pointsSet = new SortedSet<Vector2>(new CompereByStartPoint(start));
            var direction = end - start;

            var tan = direction.y / direction.x;
            var ctg = direction.x / direction.y;
            
            _intersectionAngle = Math.Atan(tan);
            
            var offsetY = end.y - tan * end.x; 
            var offsetX = end.x - ctg * end.y; 
            
            var startX = direction.x > 0 ? MathF.Ceiling(start.x) : MathF.Floor(start.x);
            var startY = direction.y > 0 ? MathF.Ceiling(start.y) : MathF.Floor(start.y);
            
            var endX = direction.x > 0 ? MathF.Ceiling(end.x) : MathF.Floor(end.x);
            var endY = direction.y > 0 ? MathF.Ceiling(end.y) : MathF.Floor(end.y);

            while (startX != endX)
            {
                var interY = tan * startX + offsetY;
                pointsSet.Add(new Vector2(startX, interY));
                startX += direction.x > 0 ? 1 : -1;
            }
            while (startY != endY)
            {
                var interX = ctg * startY + offsetX;
                pointsSet.Add(new Vector2(interX, startY));
                startY += direction.y > 0 ? 1 : -1;
            }
            return pointsSet;
        }

        private static float GetTrapezoidArea(float side1, float side2, float height = 1f)
        {
            var intersectionPlace = (side1 + side2) * height / 2;
            return MathF.Min(intersectionPlace, 1 - intersectionPlace);
        }
    }
}
