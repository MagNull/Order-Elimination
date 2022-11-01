using UnityEngine;
using System.Collections.Generic;

namespace OrderElimination
{
    public class Path : MonoBehaviour
    {
        [SerializeField] List<PlanetPoint> ends;

        private void Awake() 
        {
            gameObject.SetActive(false);
        }

        public void IncreaseEndPoint()
        {
            foreach(var end in ends)
                end.IncreasePoint();
        }

        public void DecreaseEndPoint()
        {
            foreach(var end in ends)
                end.DecreasePoint();
        }

        public PlanetPoint GetSelectedEndPoint()
        {
            foreach(var end in ends)
            {
                Debug.Log($"{end.name} {end.isSelected}");
                if(end.isSelected)
                    return end;
            }
            return null;
        }
    }
}