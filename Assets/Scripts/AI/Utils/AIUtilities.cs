using System;
using System.Linq;
using UnityEngine;

namespace AI.Utils
{
    public static class AIUtilities
    {
        public static Vector2Int[] GetCellsFromTarget(int radius, Vector2Int targetPos)
        {
            var cells = Array.Empty<Vector2Int>();
            for (var x = -radius; x <= radius; x++)
            {
                var y = Mathf.Sqrt(radius * radius - x * x);
                cells = cells.Append(new Vector2Int(x, (int)y) + targetPos).ToArray();
                cells = cells.Append(new Vector2Int(x, -(int)y) + targetPos).ToArray();
            }

            return cells;
        }   
    }
}