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
    public class SimpleMapGenerator
    {
        private readonly Transform _parent;
        private readonly Point _pointPrefab;
        private readonly IObjectResolver _resolver;
        private int _lastIndex = 0;
        private IPlayerProgressManager _progressManager;
        private const string MapPath = "Points\\RoguelikeMaps";

        [Inject]
        public SimpleMapGenerator(Point pointPrefab, Transform pointsParent,
            IObjectResolver resolver, ScenesMediator mediator)
        {
            _pointPrefab = pointPrefab;
            _parent = pointsParent;
            _resolver = resolver;
            _progressManager = mediator.Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager);
        }

        public Tuple<PointGraph, List<Point>> GenerateMap()
        {
            var map = LoadMap();
            var points = LoadPoints(map);
            return Tuple.Create(map, points);
        }

        private PointGraph LoadMap()
        {
            var maps = Resources.LoadAll<PointGraph>(MapPath);
            var loadedMapGuid = _progressManager.GetPlayerProgress().CurrentRunProgress.CurrentMapId;
            if (loadedMapGuid != Guid.Empty)
            {
                return maps.First(x => x.AssetId == loadedMapGuid);
            }

            List<PointGraph> firstMaps = new();
            foreach (var map in maps)
            {
                if (map.IsFirstMap)
                {
                    firstMaps.Add(map);
                }
            }

            return firstMaps.Count == 0
                ? GenerateRandomMap(maps)
                : GenerateRandomMap(firstMaps.ToArray());
        }

        private PointGraph GenerateRandomMap(PointGraph[] maps)
        {
            var mapIndex = Random.Range(0, maps.Length);
            _progressManager.GetPlayerProgress().CurrentRunProgress.CurrentMapId = maps[mapIndex].AssetId;
            return maps[mapIndex];
        }

        private List<Point> LoadPoints(PointGraph map)
        {
            var currentProgress = _progressManager.GetPlayerProgress().CurrentRunProgress;
            var isInitialize = currentProgress.PassedPoints.Count > 0;
            return isInitialize ? LoadSavedPoints(map) : GeneratePoints(map);
        }

        private List<Point> LoadSavedPoints(PointGraph map)
        {
            var savedPointsId = _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints.Keys.ToList();
            var savedPoint = map.GetPoints();
            return savedPoint
                            .Where(x => savedPointsId.Contains(x.AssetId))
                            .Select(x => CreatePoint(x, false))
                            .ToList();
        }

        private List<Point> GeneratePoints(PointGraph map)
        {
            List<Point> points = new();
            map.Initialize();
            points.Add(CreatePoint(map.CurrentPoint, true));
            PointModel pointModel = map.GetNextPoint();
            while (pointModel != null)
            {
                points.Add(CreatePoint(pointModel, true));
                pointModel = map.GetNextPoint();
            }
            return points;
        }

        private Point CreatePoint(PointModel pointModel, bool isNeedInitialize)
        {
            var point = _resolver.Instantiate(_pointPrefab, pointModel.position, Quaternion.identity, _parent);
            point.Initialize(pointModel, _lastIndex++);
            point.name = point.Id.ToString();
            if (isNeedInitialize)
            {
                _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints[point.Id] = pointModel is StartPointModel;
            }
            return point;
        }
    }
}
