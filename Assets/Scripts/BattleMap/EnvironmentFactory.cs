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
            Logging.LogException( new System.NotSupportedException("Had been deprecated"));
            throw new System.NotSupportedException("Had been deprecated");
        }
    }
}