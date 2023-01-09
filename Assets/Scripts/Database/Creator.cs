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
        private PlanetPoint _planetPointPrefab;
        [SerializeField]
        private Squad _squadPrefab;
        [SerializeField]
        private EnemySquad _enemySquadPrefab;
        [SerializeField]
        private Path _pathPrefab;
        [SerializeField]
        private Button _rectanglePrefab;
        [SerializeField]
        private GameObject _parent;

        private IObjectResolver _objectResolver;
        private static int num = 0;
        public static event Action<ISelectable> Created;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _objectResolver = resolver;
        }

        public PlanetPoint CreatePlanetPoint(PlanetInfo planetInfo)
        {
            var planetPoint = Instantiate(_planetPointPrefab, planetInfo.Position, Quaternion.identity, _parent.transform);
            planetPoint.name = num.ToString();
            num++;
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(Vector3 position)
        {
            var squad = _objectResolver.Instantiate(_squadPrefab, position, Quaternion.identity, _parent.transform);
            Created?.Invoke(squad);
            return squad;
        }

        public Path CreatePath()
        {
            var path = Instantiate(_pathPrefab, Vector3.zero, Quaternion.identity, _parent.transform);
            return path;
        }

        public Button CreateSquadButton(Vector3 position)
        {
            Vector3 _position = new Vector3((Screen.width / 100) * 88, position.y, 0);
            var button = Instantiate(_rectanglePrefab, _position, Quaternion.identity, _parent.transform);
            return button;
        }

        public EnemySquad CreateEnemySquad(Vector3 position)
        {
            var enemySquad = Instantiate(_enemySquadPrefab, position, Quaternion.identity, _parent.transform);
            return enemySquad;
        }
    }
}