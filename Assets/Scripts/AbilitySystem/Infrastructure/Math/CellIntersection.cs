using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public readonly struct CellIntersection
    {
        public readonly Vector2Int CellPosition; //TODO Int Only
        public readonly double IntersectionAngle;
        public readonly double SmallestPartSquare;

        public CellIntersection(Vector2Int cellPosition, double intersectionAngle, double smallestPartSquare)
        {
            CellPosition = cellPosition;
            IntersectionAngle = intersectionAngle;
            SmallestPartSquare = smallestPartSquare;
        }
    }
}
