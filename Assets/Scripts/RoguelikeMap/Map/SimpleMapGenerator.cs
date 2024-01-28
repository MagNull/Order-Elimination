using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.SavesManagement;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace RoguelikeMap.Map
{
    public class SimpleMapGenerator : IMapGenerator
    {
        private readonly Transform _parent;
        private readonly Point _pointPrefab;
        private readonly IObjectResolver _resolver;
        private readonly List<Point> _points;
        private int _lastIndex = 0;
        private IPlayerProgressManager _progressManager;

        [Inject]
        public SimpleMapGenerator(Point pointPrefab, Transform pointsParent,
            IObjectResolver resolver, ScenesMediator mediator )
        {
            _pointPrefab = pointPrefab;
            _parent = pointsParent;
            _resolver = resolver;
            _progressManager = mediator.Get<IPlayerProgressManager>("progress manager");
        }

        public List<Point> GenerateMap()
        {
            var path = "Points\\RoguelikeMaps";
            var maps = Resources.LoadAll<PointGraph>(path);
            var mapIndex = Random.Range(0, maps.Length);
            _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints = new Dictionary<Guid, bool>();
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
            _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints.Add(point.Id, false);
            return point;
        }
    }
}
