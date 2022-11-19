using UnityEngine;
using UnityEngine.UI;
using System;

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

        private Canvas _canvas;
        public static event Action<ISelectable> Created;

        private void Start()
        {
            _canvas = GameObject.Find("StrategyMapCanvas").GetComponent<Canvas>();
        }

        public PlanetPoint CreatePlanetPoint(PlanetInfo planetInfo)
        {
            var planetPoint = Instantiate(_planetPointPrefab, planetInfo.Position, Quaternion.identity);
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(SquadInfo squadInfo)
        {
            var squad = Instantiate(_squadPrefab, squadInfo.Position, Quaternion.identity);
            Created?.Invoke(squad);
            return squad;
        }

        public Path CreatePath(PathInfo pathInfo)
        {
            var path = Instantiate(_pathPrefab, pathInfo.Positon, Quaternion.identity);
            return path;
        }

        //TODO(����): ���������� �� ��������� ���������� ������(��� �������� �����)
        public Button CreateSquadButton(Vector3 position)
        {
            Vector3 _position = new Vector3((Screen.width / 100) * 88, position.y, 0);
            var button = Instantiate(_rectanglePrefab, _position, Quaternion.identity, _canvas.transform);
            button.onClick.AddListener(() => _orderPanel.SetActive());
            return button;
        }
    }
}