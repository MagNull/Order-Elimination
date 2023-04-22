using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeMap.Points
{
    public class PathView
    {
        private readonly Transform _transform;
        private readonly LineRenderer _pathPrefab;
        private readonly List<LineRenderer> _paths;

        public PathView(Transform transform, LineRenderer pathPrefab)
        {
            _transform = transform;
            _pathPrefab = pathPrefab;
            _paths = new List<LineRenderer>();
        }
        
        public void SetPath(Vector3 path)
        {
            var lineRenderer = Object.Instantiate(_pathPrefab, _transform);
            lineRenderer.SetWidth(7, 7);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetPosition(0, _transform.position);
            lineRenderer.SetPosition(1, path);
            _paths.Add(lineRenderer);
        }
        
        public void ShowPaths()
        {
            foreach (var path in _paths)
            {
                path.enabled = true;
            }
        }
    }
}