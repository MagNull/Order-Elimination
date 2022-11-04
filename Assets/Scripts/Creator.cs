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

        public PlanetPoint CreatePlanetPoint(PlanetInfo planetInfo) 
        {
            var planetPoint = GameObject.Instantiate(_planetPointPrefab, planetInfo.Position, Quaternion.identity, _canvas.transform);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(SquadInfo squadInfo)
        {
            var squad = GameObject.Instantiate(_squadPrefab, squadInfo.Position, Quaternion.identity, _canvas.transform);
            Created?.Invoke(squad); 
            return squad;
        }

        public Path CreatePath(PathInfo pathInfo)
        {
            var path = GameObject.Instantiate(_pathPrefab, pathInfo.Positon, Quaternion.identity, _canvas.transform);
            return path;
        }
    }
}

