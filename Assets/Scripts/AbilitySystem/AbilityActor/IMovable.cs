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
        public event Action<Vector2Int, Vector2Int> MovedFromTo;
        public void Move(Vector2Int destination);
    }
}
