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
        [SerializeField] private int numberOfMap = 0;
        [SerializeField] private Transform _parent;

        private void Start()
        {
            GenerateMap();
        }
        //VERY VERY COOL CRUTCH TODO: Add Paths to points 

<<<<<<< Updated upstream
        public List<IPoint> GenerateMap()
        {
            var pointsList = new List<IPoint>();
=======
        public List<PlanetPoint> GenerateMap()
        {
            var pointsList = new List<PlanetPoint>();
>>>>>>> Stashed changes
            var path = "Points\\" + numberOfMap.ToString();
            var pointsInfo = Resources.LoadAll<PointInfo>(path);
            
            Debug.Log("Load PointInfo: " + pointsInfo.Length);

            for (var i = 0; i < pointsInfo.Length; i++)
            {
<<<<<<< Updated upstream
                var info = pointsInfo[i];
                var point = CreatePoint(info);
                
=======
                var point = CreatePlanetPoint(info);
>>>>>>> Stashed changes
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
            }
            
            return pointsList;
        }

<<<<<<< Updated upstream
        private void SetPaths(ref List<IPoint> points, ref List<PointInfo> infos)
        {
            throw new NotImplementedException();
        }

        private IPoint CreatePoint(PointInfo info)
        {
            var pointObj = GameObject.Instantiate(info.Prefab, info.Position, Quaternion.identity, _parent);
            var point = pointObj.GetComponent<IPoint>();
            Debug.Log(point!=null);
            // var point = (IPlanetPoint)GetComponent(typeof(IPlanetPoint));
            point.SetPlanetInfo(info);
            return point;
        }
    }
=======
        private PlanetPoint CreatePlanetPoint(PlanetInfo info)
        {
            var pointObj = Instantiate(info.Prefab, info.Position, Quaternion.identity, _parent);
            var point = pointObj.GetComponent<IPlanetPoint>();
            //var point = (IPlanetPoint)GetComponent(typeof(IPlanetPoint));

            Debug.Log(point!=null);

            point.SetPlanetInfo(info);
            
            for (var path in point.)
            
            return point;
        }
    }   
>>>>>>> Stashed changes
}
