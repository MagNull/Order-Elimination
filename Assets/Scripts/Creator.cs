using UnityEngine;
using System;

namespace OrderElimination
{
    public class Creator : MonoBehaviour
    {
        [SerializeField] private PlanetPoint _prefabPlanetPoint;
        [SerializeField] private Squad _prefabSquad;
        public static event Action<ISelectable> Created;

        public PlanetPoint CreatePlanetPoint() 
        {
            var planetPoint = GameObject.Instantiate(_prefabPlanetPoint);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad()
        {
            var squad = GameObject.Instantiate(_prefabSquad);
            Created?.Invoke(squad); 
            return squad;
        }
    }
}

