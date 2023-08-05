using System.Collections.Generic;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoguelikeMap.Map
{
    public class SimpleMapGenerator : IMapGenerator
    {
        private const int NumberOfMap = 0;
        private readonly Transform _parent;
        private readonly Point _pointPrefab;
        private readonly LineRenderer _pathPrefab;
        private readonly IObjectResolver _resolver;

        [Inject]
        public SimpleMapGenerator(Point pointPrefab, Transform pointsParent,
            LineRenderer pathPrefab, IObjectResolver resolver)
        {
            _pointPrefab = pointPrefab;
            _parent = pointsParent;
            _pathPrefab = pathPrefab;
            _resolver = resolver;
        }

        public List<Point> GenerateMap()
        {
            var path = "Points\\Maps";
            var maps = Resources.LoadAll<PointGraph>(path);
            var mapIndex = Random.Range(0, maps.Length);
            var points = GeneratePoints(maps[mapIndex]);
            //GeneratePaths(points);
            return points;
        }

        private List<Point> GeneratePoints(PointGraph map)
        {
            var points = new List<Point>();

            foreach (var pointModel in map.GetPoints())
            {
                var newPoint = CreatePoint(pointModel);
                points.Add(newPoint);
            }
            return points;
        }

        private Point CreatePoint(PointModel pointModel)
        {
            var point = _resolver.Instantiate(_pointPrefab, pointModel.position, Quaternion.identity, _parent);
            point.Initialize(pointModel);
            return point;
        }

        private PathView GeneratePaths(List<Point> points)
        {
            return new PathView(_parent, _pathPrefab, points);
        }
    }
}
