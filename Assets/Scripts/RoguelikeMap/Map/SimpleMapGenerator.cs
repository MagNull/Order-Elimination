using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Points;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoguelikeMap.Map
{
    public class SimpleMapGenerator : IMapGenerator
    {
        private const int NumberOfMap = 0;
        private readonly Transform _parent;
        private readonly GameObject _pointPrefab;
        private readonly LineRenderer _pathPrefab;
        private readonly IObjectResolver _resolver;

        [Inject]
        public SimpleMapGenerator(GameObject pointPrefab, Transform pointsParent,
            LineRenderer pathPrefab, IObjectResolver resolver)
        {
            _pointPrefab = pointPrefab;
            _parent = pointsParent;
            _pathPrefab = pathPrefab;
            _resolver = resolver;
        }

        public List<Point> GenerateMap()
        {
            var path = "Points\\" + NumberOfMap;
            var pointsInfo = Resources.LoadAll<PointInfo>(path);
            var points = GeneratePoints(pointsInfo);
            GeneratePaths(points);
            return points;
        }

        private List<Point> GeneratePoints(IEnumerable<PointInfo> pointsInfo)
        {
            return pointsInfo.Select(CreatePoint).ToList();
        }

        private Point CreatePoint(PointInfo info)
        {
            var pointObj = _resolver.Instantiate(_pointPrefab, info.Model.Position, Quaternion.identity, _parent);

            var point = pointObj.GetComponent<Point>();
            point.SetPointModel(info.Model, info.PointSprite);
            
            return point;
        }

        private PathView GeneratePaths(List<Point> points)
        {
            return new PathView(_parent, _pathPrefab, points);
        }
    }
}
