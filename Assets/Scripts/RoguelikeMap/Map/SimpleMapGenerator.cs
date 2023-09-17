using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoguelikeMap.Map
{
    public class SimpleMapGenerator : IMapGenerator
    {
        private readonly Transform _parent;
        private readonly Point _pointPrefab;
        private readonly IObjectResolver _resolver;
        private readonly List<Point> _points;
        private int _lastIndex = 0;

        [Inject]
        public SimpleMapGenerator(Point pointPrefab, Transform pointsParent,
            IObjectResolver resolver)
        {
            _pointPrefab = pointPrefab;
            _parent = pointsParent;
            _resolver = resolver;
        }

        public List<Point> GenerateMap()
        {
            var path = "Points\\RoguelikeMaps";
            var maps = Resources.LoadAll<PointGraph>(path);
            var mapIndex = Random.Range(0, maps.Length);
            var points = GeneratePoints(maps[mapIndex]);
            return points;
        }

        private List<Point> GeneratePoints(PointGraph map)
        {
            return map.GetPoints().Select(CreatePoint).ToList();
        }

        private Point CreatePoint(PointModel pointModel)
        {
            var point = _resolver.Instantiate(_pointPrefab, pointModel.position, Quaternion.identity, _parent);
            point.Initialize(pointModel, _lastIndex++);
            return point;
        }
    }
}
