using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class CellMath
    {
        public static CellIntersection[] GetIntersectionBetween(Vector2Int startPoint, Vector2Int endPoint)
        {
            return IntersectionSolver.GetIntersections(startPoint, endPoint).ToArray();
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
            //return Enumerable.Empty<CellIntersection>();
            throw new NotImplementedException();
        }

        public static float GetRealDistanceBetween(Vector2Int posA, Vector2Int posB)
        {
            return (posB - posA).magnitude;
        }

        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            var devidend = value - a;
            var devider = b - a;
            if (devidend == 0) return 0;
            return devidend / devider;
        }

        public static Vector2Int[] GetNeighbours(Vector2Int position)
        {
            var neighbours = new List<Vector2Int>();
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    neighbours.Add(position + new Vector2Int(dx, dy));
                }
            }
            return neighbours.ToArray();
        }

        public static Vector2Int GetPositionByPriority(
            this CellPriority priority, Vector2Int[] positions, Vector2Int? casterPos, Vector2Int? targetPos)
        {
            if (priority == CellPriority.FirstInGroup)
                return positions.First();
            if (priority == CellPriority.LastInGroup)
                return positions.Last();

            var sortedByCasterPositions = positions.OrderBy(p => (p - casterPos.Value).sqrMagnitude);
            if (priority == CellPriority.ClosestToCaster)
                return sortedByCasterPositions.First();
            if (priority == CellPriority.FurthestFromCaster)
                return sortedByCasterPositions.Last();

            var sortedByTargetPositions = positions.OrderBy(p => (p - targetPos.Value).sqrMagnitude);
            if (priority == CellPriority.ClosestToTarget)
                return sortedByTargetPositions.First();
            if (priority == CellPriority.FurthestFromTarget)
                return sortedByTargetPositions.Last();

            throw new NotImplementedException();
        }
    }
}
