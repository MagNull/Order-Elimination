using Sirenix.OdinInspector;
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

    [Serializable]
    public class PointRelativePattern
    {
        public IEnumerable<Vector2Int> RelativePositions => _relativePositions;
        [ShowInInspector, SerializeField]
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

        public static float GetRealDistanceBetween(Vector2Int posA, Vector2Int posB)
        {
            return (posB - posA).magnitude;
        }

        //public static float GetInterpolatedValue(float value, float minValue, float maxValue)
        //{

        //}
    }
}
