using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class Planet
    {
        private List<PlanetPoint> _points;
        private string _name;

        public Planet(string name)
        {
            _name = name;
            _points = new List<PlanetPoint>();
        }

        public void AddPoint(PlanetPoint point)
        {
            _points.Add(point);
        }

        public void RemovePoint(PlanetPoint point)
        {
            _points.Remove(point);
        }
    }
}
