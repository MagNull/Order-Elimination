using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoguelikeMap.Points
{
    public class PathView
    {
        private readonly LineRenderer _paths;
        private int _lastIndex;

        public PathView(Transform transform, LineRenderer pathPrefab, List<Point> points)
        {
            if (!points.Any())
                return;

            _paths = Object.Instantiate(pathPrefab, transform);
            _paths.SetWidth(7, 7);
            _paths.material = new Material(Shader.Find("Sprites/Default"));
            _lastIndex = 0;
            _paths.positionCount = points.Count * points.Count;
            
            SetPaths(points, new []{0}, -1);
        }

        private void SetPaths(List<Point> points, IReadOnlyList<int> indexes, int prevIndex)
        {
            if (indexes is null || indexes.Count == 0)
                return;
            foreach (var index in indexes)
            {
                var point = points[index];
                var position = point.transform.position;
                SetPosition(position);
                SetPaths(points, point.NextPoints, index);
                if(prevIndex < 0)
                    continue;
                var prevPosition = points[prevIndex].transform.position;
                SetPosition(prevPosition);
            }
        }
        
        private void SetPosition(Vector3 path)
        {
            _paths.SetPosition(_lastIndex++, path);
        }
    }
}