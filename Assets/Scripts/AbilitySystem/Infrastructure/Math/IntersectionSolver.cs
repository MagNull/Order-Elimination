using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
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
        private const int RoundAccuracy = 6;
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

            var isPoint1X = point1.x % 1 == 0;
            var isPoint2X = point2.x % 1 == 0;

            if (isPoint1X && isPoint2X)
                interArea = GetTrapezoidArea(point1.x, point2.x, 1f);
            else if (!isPoint1X && !isPoint2X)
                interArea = GetTrapezoidArea(point1.y, point2.y, 1f);
            else if (isPoint1X && !isPoint2X)
            {
                var sideVert = point2.y == cellPos.y ? point1.y : 1 - point1.y;
                var sideHoriz = point1.x == cellPos.x ? point2.x : 1 - point2.x;
                interArea = GetTrapezoidArea(sideVert, 0f, sideHoriz);
            }
            else
            {
                var sideVert = point1.y == cellPos.y ? point2.y : 1 - point2.y;
                var sideHoriz = point2.x == cellPos.x ? point1.x : 1 - point1.x;
                interArea = GetTrapezoidArea(sideVert, 0f, sideHoriz);
            }
            var intVector = new Vector2Int((int)cellPos.x, (int)cellPos.y);
            return new CellIntersection(intVector, _intersectionAngle, interArea);
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
                resultDict.Add(cellPosition, new Tuple<Vector2, Vector2>(previousPoint, currentPoint));
                previousPoint = currentPoint;
            }

            return resultDict;
        }

        private static SortedSet<Vector2> GetIntersectionPoints(Vector2 start, Vector2 end)
        {
            var pointsSet = new SortedSet<Vector2>(new CompereByStartPoint(start));
            var direction = end - start;
            float tan;
            if (direction.x == 0)
                tan = direction.y > 0 ? float.MaxValue : float.MinValue;
            else
                tan = direction.y / direction.x;
            _intersectionAngle = Math.Atan(tan);
            var offsetY = start.y - start.x * tan;

            var startX = direction.x > 0 ? MathF.Ceiling(start.x) : MathF.Floor(start.x);
            var startY = direction.y > 0 ? MathF.Ceiling(start.y) : MathF.Floor(start.y);

            var endX = direction.x > 0 ? MathF.Ceiling(end.x) : MathF.Floor(end.x);
            var endY = direction.y > 0 ? MathF.Ceiling(end.y) : MathF.Floor(end.y);

            while (startX != endX || startY != endY)
            {
                if (startX != endX)
                {
                    var interY = MathF.Round((startX * tan) + offsetY, RoundAccuracy);
                    pointsSet.Add(new Vector2(startX, interY));
                    startX = direction.x > 0 ? startX + 1 : startX - 1;
                }
                if (startY != endY)
                {
                    var interX = MathF.Round((startY - offsetY) / tan, RoundAccuracy);
                    pointsSet.Add(new Vector2(interX, startY));
                    startY = direction.y > 0 ? startY + 1 : startY - 1;
                }
            }

            return pointsSet;
        }

        private static float GetTrapezoidArea(float side1, float side2, float height)
        {
            var intersectionPlace = (side1 + side2) * height / 2;
            return MathF.Min(intersectionPlace, 1 - intersectionPlace);
        }
    }

}
