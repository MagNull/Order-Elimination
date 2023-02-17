using System;
using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

namespace OrderElimination
{
    public class SimpleMapGenerator : MonoBehaviour, IMapGenerator
    {
        [SerializeField] private int numberOfMap = 0;
        [SerializeField] private Transform _parent;

        private void Start()
        {
            GenerateMap();
        }
        //VERY VERY COOL CRUTCH TODO: Add Paths to points 

        public List<IPoint> GenerateMap()
        {
            var pointsList = new List<IPoint>();
            var path = "Points\\" + numberOfMap.ToString();
            var pointsInfo = Resources.LoadAll<PlanetInfo>(path);
            
            Debug.Log("Load PointInfo: " + pointsInfo.Length);

            foreach (var info in pointsInfo)
            {
                var pointObj = Instantiate(info.Prefab, info.Position, Quaternion.identity, _parent);
                var point = pointObj.GetComponent<IPoint>();
                Debug.Log(point!=null);
                // var point = (IPlanetPoint)GetComponent(typeof(IPlanetPoint));
                pointsList.Add(point);
            }
            Debug.Log("Generate points: " + pointsList.Count);
            return pointsList;
        }
    }
}
