using UnityEngine;
using System;

namespace OrderElimination
{
    public class Creator : MonoBehaviour
    {
        [SerializeField] private PlanetPoint _planetPointPrefab;
        [SerializeField] private Squad _squadPrefab;
        [SerializeField] private Path _pathPrefab;
        public static event Action<ISelectable> Created;

        public PlanetPoint CreatePlanetPoint(Vector3 positionOnMap) 
        {
            var planetPoint = GameObject.Instantiate(_planetPointPrefab, positionOnMap, Quaternion.identity);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(Vector3 positionOnMap)
        {
            var squad = GameObject.Instantiate(_squadPrefab, positionOnMap, Quaternion.identity);
            Created?.Invoke(squad); 
            return squad;
        }

        public Path CreatePath(Vector3 positionOnMap)
        {
            var path = GameObject.Instantiate(_pathPrefab, positionOnMap, Quaternion.identity);
            return path;
        }
    }
}

