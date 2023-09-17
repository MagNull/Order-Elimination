using System.Collections;
using System.Collections.Generic;
using RoguelikeMap.Points.Models;
using UnityEngine;

namespace RoguelikeMap.Points
{
    public class PathView : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _pathPrefab;
        private readonly List<LineRenderer> _paths = new ();
        private const int _duration = 2;

        public void UpdatePaths(Point point)
        {
            if (_paths.Count != 0)
                ClearPaths();
            var nextPoints = point.Model.GetNextPoints();
            foreach (var model in nextPoints)
            {
                var lineRenderer = Instantiate(_pathPrefab, transform);
                lineRenderer.SetPosition(0, point.Model.position);
                lineRenderer.SetPosition(1, model.position);
                StartCoroutine(AnimateLine(lineRenderer));
                _paths.Add(lineRenderer);
            }
        }

        private IEnumerator AnimateLine(LineRenderer lineRenderer)
        {
            var pointsCount = lineRenderer.positionCount;
            float segmentDuration = _duration / pointsCount;
            for (var i = 0; i < pointsCount - 1; i++)
            {
                var startTime = Time.time;
                var startPosition = lineRenderer.GetPosition(i);
                var endPosition = lineRenderer.GetPosition(i + 1);

                var currentPosition = startPosition;
                while (currentPosition != endPosition)
                {
                    var t = (Time.time - startTime) / segmentDuration;
                    currentPosition = Vector3.Lerp(startPosition, endPosition, t);
                    for(var j = i + 1; j < pointsCount; j++)
                        lineRenderer.SetPosition(j, currentPosition);

                    yield return null;
                }
            }
        }

        public void ClearPaths()
        {
            foreach (var path in _paths)
                Destroy(path.gameObject);
        }
    }
}