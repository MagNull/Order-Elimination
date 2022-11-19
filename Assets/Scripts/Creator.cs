using UnityEngine;
using UnityEngine.UI;
using System;

namespace OrderElimination
{
    public class Creator : MonoBehaviour
    {
        [SerializeField] private PlanetPoint _planetPointPrefab;
        [SerializeField] private Squad _squadPrefab;
        [SerializeField] private Path _pathPrefab;
        [SerializeField] private Button _buttonPrefab;

        private Canvas _canvas;
        public static event Action<ISelectable> Created;

        private void Start()
        {
            _canvas = GameObject.Find("StrategyMapCanvas").GetComponent<Canvas>();
        }

        public PlanetPoint CreatePlanetPoint(PlanetInfo planetInfo) 
        {
            var planetPoint = GameObject.Instantiate(_planetPointPrefab, planetInfo.Position, Quaternion.identity);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(SquadInfo squadInfo)
        {
            var squad = GameObject.Instantiate(_squadPrefab, squadInfo.Position, Quaternion.identity);
            Created?.Invoke(squad);
            return squad;
        }

        public Path CreatePath(PathInfo pathInfo)
        {
            var path = GameObject.Instantiate(_pathPrefab, pathInfo.Positon, Quaternion.identity);
            return path;
        }

        public Button CreateSquadButton(Vector3 position)
        {
            var button = GameObject.Instantiate(_buttonPrefab, position, Quaternion.identity, _canvas.transform) as Button;
            return button;
        }
    }
}

