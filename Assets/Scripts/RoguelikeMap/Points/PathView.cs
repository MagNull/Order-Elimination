using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

namespace RoguelikeMap.Points
{
    public class PathView
    {
        private readonly LineRenderer _paths;
        private int _lastIndex;

        public PathView(Transform transform, LineRenderer pathPrefab)
        {
            _paths = Object.Instantiate(pathPrefab, transform);
            _paths.SetWidth(7, 7);
            _paths.material = new Material(Shader.Find("Sprites/Default"));
        }

        public void UpdatePaths(Point point)
        {
            ClearLines();
            _lastIndex = 0;
            var nextPoints = point.Model.GetNextPoints();
            _paths.positionCount = nextPoints.Count() * 2;
            foreach(var nextPoint in nextPoints)
            {
                SetPosition(point.transform.position);
                SetPosition(nextPoint.position);
            }
        }
        
        private void SetPosition(Vector3 path)
        {
            _paths.SetPosition(_lastIndex++, path);
        }

        private void ClearLines()
        {
            _paths.positionCount = 0;
        }
    }
}