using UnityEngine;
using System;

namespace OrderElimination
{
    public class Creator : MonoBehaviour
    {
        [SerializeField] private PlanetPoint _planetPointPrefab;
        [SerializeField] private Squad _squadPrefab;
        [SerializeField] private Path _pathPrefab;
        private Canvas _canvas;
        public static event Action<ISelectable> Created;

        private void Start()
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }

        public PlanetPoint CreatePlanetPoint(Vector3 positionOnMap) 
        {
            var planetPoint = Instantiate(_planetPointPrefab, positionOnMap, Quaternion.identity, _canvas.transform);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(Vector3 positionOnMap)
        {
            var squad = Instantiate(_squadPrefab, positionOnMap, Quaternion.identity, _canvas.transform);
            Created?.Invoke(squad); 
            return squad;
        }

        public Path CreatePath(Vector3 positionOnMap)
        {
            var path = Instantiate(_pathPrefab, positionOnMap, Quaternion.identity, _canvas.transform);
            return path;
        }
    }
}

