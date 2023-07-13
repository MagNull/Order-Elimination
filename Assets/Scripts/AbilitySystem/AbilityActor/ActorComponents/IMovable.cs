using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IMovable
    {
        public Vector2Int Position { get; }
        public bool CanMove { get; }
        //public bool CanMoveTo(Vector2Int destination, bool forceMove = false);
        public bool Move(Vector2Int destination, bool forceMove = false);

        public event Action<Vector2Int, Vector2Int> MovedFromTo;
    }
}
