using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OrderElimination
{
    public class SimpleMapGenerator : IMapGenerator
    {
        private readonly int _numberOfMap;
        private Transform _parent;

        public SimpleMapGenerator(int numberOfMap, Transform parent)
        {
            _numberOfMap = numberOfMap;
            _parent = parent;
        }

        public List<Point> GenerateMap()
        {
            // Load PointInfo
            var pointsList = new List<Point>();
            var path = "Points\\" + _numberOfMap;
            var pointsInfo = Resources.LoadAll<PointInfo>(path);
            
            // Generate points
            for (var i = 0; i < pointsInfo.Length; i++)
            {
                var info = pointsInfo[i];
                var point = CreatePoint(info);
                
                pointsList.Add(point);
                point.PointNumber = i;
            }
            
            // Initialize paths
            foreach (var info in pointsInfo)
            {
                var p = pointsList
                    .First(x => x.PointInfo == info);
                if (p != null)
                    p.SetNextPoints(pointsList
                        .Where(x => (info.NextPoints
                            .Contains(x.PointInfo))));
                
                p.ShowPaths();
            }
            
            return pointsList;
        }

        private Point CreatePoint(PointInfo info)
        {
            var pointObj = Object.Instantiate(info.Prefab, info.Position, Quaternion.identity, _parent);
            var point = pointObj.GetComponent<Point>();
            //Debug.Log(point.HasEnemy);
            point.SetPlanetInfo(info);
            return point;
        }
    }
}
