using UnityEngine;
using System.Collections.Generic;

namespace OrderElimination
{
    public class Path : MonoBehaviour
    {
        private List<PlanetPoint> _ends;

        private void Awake() 
        {
            _ends = new List<PlanetPoint>();
        }
        
        public void SetEndPoint(PlanetPoint planetPoint)
        {
            Debug.Log("SetEndPoint");
            _ends.Add(planetPoint);
        }

        public void IncreaseEndPoint()
        {
            foreach(var end in _ends)
                end.IncreasePoint();
        }

        public void DecreaseEndPoint()
        {
            foreach(var end in _ends)
                end.DecreasePoint();
        }
    }
}