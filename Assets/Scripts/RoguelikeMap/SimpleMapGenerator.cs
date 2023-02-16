using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

namespace OrderElimination
{
    public class SimpleMapGenerator : MonoBehaviour, IMapGenerator
    {
        [SerializeField] private int numberOfMap = 0;
        public List<PlanetPoint> GenerateMap()
        {
            throw new System.NotImplementedException();
        }
    }   
}
