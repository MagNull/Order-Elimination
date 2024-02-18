using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.SavesManagement;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using Unity.VisualScripting;
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
            IObjectResolver resolver, ScenesMediator mediator)
        {
            _pointPrefab = pointPrefab;
            _parent = pointsParent;
            _resolver = resolver;
            _progressManager = mediator.Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager);
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
            var isInitialize = _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints.Count == 0;
            List<Point> points = new();
            PointModel pointModel = map.GetNextPoint();
            while (pointModel is not FinalBattlePointModel)
            {
                if (pointModel == null)
                {
                    Debug.LogException(new Exception("PointModel isn't connected"));
                    break;
                }
                points.Add(CreatePoint(pointModel, isInitialize));
                pointModel = map.GetNextPoint();
            }
            return points;
        }

        private Point CreatePoint(PointModel pointModel, bool isInitialize)
        {
            var point = _resolver.Instantiate(_pointPrefab, pointModel.position, Quaternion.identity, _parent);
            point.Initialize(pointModel, _lastIndex++);
            point.name = point.Id.ToString();
            if (isInitialize)
            {
                _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints[point.Id] = pointModel is StartPointModel;
            }
            return point;
        }
    }
}
