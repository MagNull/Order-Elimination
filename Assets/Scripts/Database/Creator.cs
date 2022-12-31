using UnityEngine;
using UnityEngine.UI;
using System;
using VContainer;
using VContainer.Unity;

namespace OrderElimination
{
    public class Creator : MonoBehaviour
    {
        [SerializeField]
        private OrderPanel _orderPanel;
        [SerializeField]
        private PlanetPoint _planetPointPrefab;
        [SerializeField]
        private Squad _squadPrefab;
        [SerializeField]
        private Path _pathPrefab;
        [SerializeField]
        private Button _rectanglePrefab;

        [SerializeField]
        private Canvas _strategyMapCanvas;
        private IObjectResolver _objectResolver;
        public static event Action<ISelectable> Created;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _objectResolver = resolver;
        }

        public PlanetPoint CreatePlanetPoint(PlanetInfo planetInfo)
        {
            var planetPoint = Instantiate(_planetPointPrefab, planetInfo.Position, Quaternion.identity);
            planetPoint.SetInfo(planetInfo);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(Vector3 position)
        {
            var squad = _objectResolver.Instantiate(_squadPrefab, position, Quaternion.identity);
            Created?.Invoke(squad);
            return squad;
        }

        public Path CreatePath(PathInfo pathInfo)
        {
            var path = Instantiate(_pathPrefab, pathInfo.Positon, Quaternion.identity);
            return path;
        }

        public Button CreateSquadButton(Vector3 position)
        {
            Vector3 _position = new Vector3((Screen.width / 100) * 88, position.y, 0);
            var button = Instantiate(_rectanglePrefab, _position, Quaternion.identity, _strategyMapCanvas.transform);
            button.onClick.AddListener(() => _orderPanel.SetActive());
            return button;
        }
    }
}