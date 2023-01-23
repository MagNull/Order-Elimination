using UnityEngine;
using VContainer;

namespace OrderElimination.BM
{
    public class EnvironmentFactory
    {
        private readonly GameObject _viewPrefab;
        private readonly BattleMap _battleMap;
        private int _indexCounter = 0;

        [Inject]
        public EnvironmentFactory(GameObject viewPrefab, BattleMap battleMap)
        {
            _viewPrefab = viewPrefab;
            _battleMap = battleMap;
        }

        public EnvironmentObject Create(EnvironmentInfo environmentInfo, int lifeTime = 999)
        {
            var view = Object.Instantiate(_viewPrefab).GetComponent<EnvironmentObjectView>();
            view.GameObject.name = "Environment Object " + _indexCounter++;
            var environmentObject = new EnvironmentObject(environmentInfo.EnterEffects, view, environmentInfo.Stats,
                environmentInfo.IsWalkable, _battleMap, lifeTime);
            view.Init(environmentObject, environmentInfo.SpriteView);
            environmentObject.View = view;

            return environmentObject;
        }
    }
}