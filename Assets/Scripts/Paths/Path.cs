using UnityEngine;
using System.Collections.Generic;

namespace OrderElimination
{
    public class Path : MonoBehaviour
    {
        private PlanetPoint _start;
        private PlanetPoint _end;
        public PlanetPoint Start => _start;
        public PlanetPoint End => _end;

        public void SetStartPoint(PlanetPoint planetPoint)
        {
            Debug.Log("SetStartPoint");
            _start = planetPoint;
        }

        public void SetEndPoint(PlanetPoint planetPoint)
        {
            Debug.Log("SetEndPoint");
            _end = planetPoint;
        }

        public void Increase()
        {
            _start.IncreasePoint();
            _end.IncreasePoint();
        }

        public void Decrease()
        {
            _start.DecreasePoint();
            _end.DecreasePoint();
        }
    }
}