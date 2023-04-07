using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class PathView
    {
        private readonly Transform _transform;
        private readonly GameObject _pathPrefab;
        private readonly List<LineRenderer> _paths;

        public PathView(Transform transform, GameObject pathPrefab)
        {
            _transform = transform;
            _pathPrefab = pathPrefab;
            _paths = new List<LineRenderer>();
        }
        
        public void SetPath(Vector3 path)
        {
            var line = Object.Instantiate(_pathPrefab, _transform);
            var lineRenderer = line.GetComponent<LineRenderer>();
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