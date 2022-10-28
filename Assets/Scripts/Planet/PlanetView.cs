using UnityEngine;
using System;

namespace OrderElimination
{
    public class PlanetView  : MonoBehaviour
    {
        private GameObject _planetPoint;

        public Vector2Int GetPoisitionOnMap()
        {
            Vector3 position = _planetPoint.gameObject.transform.position;
            return new Vector2Int(Convert.ToInt32(position.x), Convert.ToInt32(position.y));
        }
    }
}