using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;

namespace OrderElimination
{
    public class SimpleMapGenerator : IMapGenerator
    {
        public int numberOfMap { get; set; }
        private Transform _parent;
        // TODO: Fix null reference
        public List<Point> GenerateMap()
        {
            var pointsList = new List<Point>();
            var path = "Points\\" + numberOfMap.ToString();
            var pointsInfo = Resources.LoadAll<PointInfo>(path);
            
            Debug.Log("Load PointInfo: " + pointsInfo.Length);

            for (var i = 0; i < pointsInfo.Length; i++)
            {
                var info = pointsInfo[i];
                var point = CreatePoint(info);
                
                pointsList.Add(point);
                point.PointNumber = i;
            }
            Debug.Log("Generate points: " + pointsList.Count);
            
            foreach (var info in pointsInfo)
            {
                pointsList
                    .First(x => x.GetPlanetInfo() == info)
                    .SetNextPoints(pointsList
                        .Where(x => (info.NextPoints
                            .Contains(x.GetPlanetInfo()))));
                Debug.Log("Next points: " + pointsList[0].NextPoints.Count);
            }
            
            return pointsList;
        }

        private void SetPaths(ref List<Point> points, ref List<PointInfo> infos)
        {
            throw new NotImplementedException();
        }

        private Point CreatePoint(PointInfo info)
        {
            var pointObj = GameObject.Instantiate(info.Prefab, info.Position, Quaternion.identity, _parent);
            var point = pointObj.GetComponent<Point>();
            //var point = (Point)pointObj.GetComponent(typeof(Point));
            Debug.Log(point.HasEnemy);
            point.SetPlanetInfo(info);
            return point;
        }
    }
}
