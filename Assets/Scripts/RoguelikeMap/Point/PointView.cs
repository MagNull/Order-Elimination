using System.Collections.Generic;
using UnityEngine;


namespace OrderElimination
{
    public class PointView
    {
        private readonly Transform _transform;
        private readonly GameObject _pathPrefab;
        private readonly List<LineRenderer> _paths;

        public PointView(Transform transform, GameObject pathPrefab)
        {
            _transform = transform;
            _pathPrefab = pathPrefab;
            _paths = new List<LineRenderer>();
        }

        public void Increase()
        {
            _transform.localScale += new Vector3(0.1f, 0.1f, 0);
        }

        public void Decrease()
        {
            _transform.localScale -= new Vector3(0.1f, 0.1f, 0);
        }

        public void SetPath(Vector3 path)
        {
            var line = Object.Instantiate(_pathPrefab, _transform);
            var lineRenderer = line.GetComponent<LineRenderer>();
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

        public void HidePaths()
        {
            foreach (var path in _paths)
            {
                path.enabled = false;
            }
        }
    }
}