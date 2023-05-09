using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IMovable
    {
        public Vector2Int Position { get; }
        public bool Move(Vector2Int destination, bool forceMove = false);

        public event Action<Vector2Int, Vector2Int> MovedFromTo;
    }
}
