using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public interface IMapGenerator
    {
        public List<PlanetPoint> GenerateMap();
    }
}
