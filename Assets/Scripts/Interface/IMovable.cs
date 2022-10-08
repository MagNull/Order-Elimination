using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public interface IMovable
    {
        public void Move(Vector2Int position);
    }

}