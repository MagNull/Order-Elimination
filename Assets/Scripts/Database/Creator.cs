using UnityEngine;
using System;
using Unity.VisualScripting;
using VContainer;
using VContainer.Unity;
using UIManagement.Elements;

namespace OrderElimination
{
    public class Creator : MonoBehaviour
    {
        [SerializeField]
        private PlanetPoint _planetPointPrefab;
        [SerializeField]
        private Squad _firstSquadPrefab;
        [SerializeField]
        private Squad _secondSquadPrefab;
        [SerializeField]
        private EnemySquad _enemySquadPrefab;
        [SerializeField]
        private EnemySquad _tutorialEnemySquadPrefab;
        [SerializeField]
        private Path _pathPrefab;
        [SerializeField]
        private HoldableButton _firstButtonSquadPrefab;
        [SerializeField]
        private HoldableButton _secondButtonSquadPrefab;
        [SerializeField]
        private GameObject _parent;

        private IObjectResolver _objectResolver;
        private int _num = 0;
        public static event Action<ISelectable> Created;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _objectResolver = resolver;
            _num = 0;
        }

        public PlanetPoint CreatePlanetPoint(PlanetInfo planetInfo)
        {
            var planetPoint = Instantiate(_planetPointPrefab, planetInfo.Position, Quaternion.identity, _parent.transform);
            planetPoint.name = _num.ToString();
            _num++;
            Created?.Invoke(planetPoint);
            return planetPoint;
        }

        public Squad CreateSquad(Vector3 position, bool isFirstSquad)
        {
            var squad = _objectResolver.Instantiate(isFirstSquad ? _firstSquadPrefab : _secondSquadPrefab, position, Quaternion.identity, _parent.transform);
            Created?.Invoke(squad);
            return squad;
        }

        public Path CreatePath()
        {
            var path = Instantiate(_pathPrefab, Vector3.zero, Quaternion.identity, _parent.transform);
            return path;
        }

        public HoldableButton CreateSquadButton(Vector3 position, bool isFirstSquad)
        {
            var button = Instantiate(isFirstSquad ? _firstButtonSquadPrefab : _secondButtonSquadPrefab, position, Quaternion.identity, _parent.transform);
            return button;
        }

        public EnemySquad CreateEnemySquad(Vector3 position)
        {
            if (!_enemySquadPrefab.IsDestroyed() && !_parent.IsDestroyed())
            {
                var enemySquad = Instantiate(_enemySquadPrefab, position, Quaternion.identity, _parent.transform);
                return enemySquad;
            }

            return null;
        }
        
        public EnemySquad CreateTutorialEnemySquad(Vector3 position)
        {
            if (!_enemySquadPrefab.IsDestroyed() && !_parent.IsDestroyed())
            {
                var enemySquad = Instantiate(_tutorialEnemySquadPrefab, position, Quaternion.identity, _parent.transform);
                return enemySquad;
            }

            return null;
        }
    }
}