using System;
using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace RoguelikeMap.Map
{
    public class SimpleMapGenerator : IMapGenerator
    {
        private const int NumberOfMap = 0;
        private readonly Transform _parent;
        private readonly PanelGenerator _panelGenerator;
        private readonly GameObject _pointPrefab;
        private readonly LineRenderer _pathPrefab;

        [Inject]
        public SimpleMapGenerator(GameObject pointPrefab, PanelGenerator panelGenerator, 
            Transform pointsParent, LineRenderer pathPrefab)
        {
            _pointPrefab = pointPrefab;
            _panelGenerator = panelGenerator;
            _parent = pointsParent;
            _pathPrefab = pathPrefab;
        }

        public IEnumerable<Point> GenerateMap()
        {
            var path = "Points\\" + NumberOfMap;
            var pointsInfo = Resources.LoadAll<PointInfo>(path);
            var points = GeneratePoints(pointsInfo);
            return points;
        }

        private IEnumerable<Point> GeneratePoints(IEnumerable<PointInfo> pointsInfo)
        {
            return pointsInfo.Select(CreatePoint).ToList();
        }

        private Point CreatePoint(PointInfo info)
        {
            var pointObj = Object.Instantiate(_pointPrefab, info.Position, Quaternion.identity, _parent);
            
            var pointSprite = pointObj.GetComponent<SpriteRenderer>();
            pointSprite.sprite = info.PointSprite;

            var point = pointObj.GetComponent<Point>();
            point.SetPanelGenerator(_panelGenerator);
            point.SetPointModel(info.Model);
            
            return point;
        }

        private PathView GeneratePaths(List<Point> points)
        {
            throw new NotImplementedException("Not implemented yet");
        }
    }
}
