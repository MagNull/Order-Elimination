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
        private readonly int _numberOfMap = 0;
        private Transform _parent;
        private PanelGenerator _panelGenerator;
        private Point _pointPrefab;

        [Inject]
        public SimpleMapGenerator(Point pointPrefab, PanelGenerator panelGenerator, Transform pointsParent)
        {
            _pointPrefab = pointPrefab;
            _panelGenerator = panelGenerator;
            _parent = pointsParent;
        }

        public IEnumerable<Point> GenerateMap()
        {
            // Load PointInfo
            var pointsList = new List<(Point, PointInfo)>();
            var path = "Points\\" + _numberOfMap;
            var pointsInfo = Resources.LoadAll<PointInfo>(path);
            
            // Generate points
            for (var i = 0; i < pointsInfo.Length; i++)
            {
                var info = pointsInfo[i];
                var point = CreatePoint(info);
                
                pointsList.Add((point, info));
                point.PointNumber = i;
            }
            
            // Initialize paths
            foreach (var info in pointsInfo)
            {
                var point = pointsList.First(x => x.Item2 == info);
                if (point.Item1 != null)
                {
                    var nextPointsInfo = info.NextPoints;
                    var nextPoints = pointsList
                        .Where(x => nextPointsInfo.Contains(x.Item2))
                        .Select(x => x.Item1);
                    point.Item1.SetNextPoints(nextPoints);
                }
                
                point.Item1.ShowPaths();
            }
            
            return pointsList.Select(x => x.Item1);
        }

        private Point CreatePoint(PointInfo info)
        {
            var pointObj = Object.Instantiate(_pointPrefab, info.Position, Quaternion.identity, _parent);
            
            var pointSprite = pointObj.GetComponent<SpriteRenderer>();
            pointSprite.sprite = info.PointSprite;
            
            var point = pointObj.GetComponent<Point>();
            point.SetPointInfo(info.VarietiesPoint);
            point.SetPanelGenerator(_panelGenerator);
            return point;
        }
    }
}
