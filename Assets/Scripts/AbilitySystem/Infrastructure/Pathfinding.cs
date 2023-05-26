using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class Pathfinding
    {
        public static bool PathExists(
            Vector2Int origin, 
            Vector2Int destination, 
            CellRangeBorders borders,
            Predicate<Vector2Int> positionPredicate, out Vector2Int[] path)
        {
            var result = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var parents = new Dictionary<Vector2Int, Vector2Int>();
            queue.Enqueue(origin);
            visited.Add(origin);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == destination)
                {
                    while (current != origin)
                    {
                        result.Add(current);
                        current = parents[current];
                    }
                    result.Reverse();
                    path = result.ToArray();
                    return true;
                }

                foreach (var neighbour in GetNeighbours(current, borders))
                {
                    if (visited.Contains(neighbour) || !positionPredicate(neighbour))
                        continue;

                    visited.Add(neighbour);
                    queue.Enqueue(neighbour);
                    parents.Add(neighbour, current);
                }
            }
            path = result.ToArray();
            return false;
        }

        private static Vector2Int[] GetNeighbours(Vector2Int position, CellRangeBorders borders)
        {
            var neighbours = new List<Vector2Int>();
            if (position.x > 0)
                neighbours.Add(new Vector2Int(position.x - 1, position.y));
            if (position.x <= borders.xMax)
                neighbours.Add(new Vector2Int(position.x + 1, position.y));
            if (position.y > 0)
                neighbours.Add(new Vector2Int(position.x, position.y - 1));
            if (position.y <= borders.yMax)
                neighbours.Add(new Vector2Int(position.x, position.y + 1));
            if (position.y <= borders.yMax && position.x < borders.xMax)
                neighbours.Add(new Vector2Int(position.x + 1, position.y + 1));
            if (position.y > 0 && position.x > 0)
                neighbours.Add(new Vector2Int(position.x - 1, position.y - 1));
            if (position.y > 0 && position.x < borders.xMax)
                neighbours.Add(new Vector2Int(position.x + 1, position.y - 1));
            if (position.y <= borders.yMax && position.x > 0)
                neighbours.Add(new Vector2Int(position.x - 1, position.y + 1));
            return neighbours.Where(p => borders.Contains(p)).ToArray();
            //return CellMath.GetNeighbours(position).Where(p => borders.Contains(p)).ToArray();
        }
    }
}
