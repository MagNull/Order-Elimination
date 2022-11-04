using System;
using CharacterAbility;
using UnityEngine;

namespace OrderElimination
{
    [Serializable]
    public class Character : ISquadMember, IBattleCharacterInfo
    {
        [SerializeField]
        private AbilityInfo[] _abilities;
        [SerializeField]
        private BattleStats _battleStats;
        private StrategyStats _strategyStats;
        
        public void Select()
        {
            throw new System.NotImplementedException();
        }

        public void Unselect()
        {
            throw new System.NotImplementedException();
        }

        public void Move(PlanetPoint planetPoint)
        {
            throw new System.NotImplementedException();
        }

        public StrategyStats GetStrategyStats() => _strategyStats;

        public BattleStats GetBattleStats() => _battleStats;

        public BattleCharacterView GetView()
        {
            throw new System.NotImplementedException();
        }

        public AbilityInfo[] GetAbilityInfos() => _abilities;

        public void RaiseExperience(float experience)
        {
            throw new System.NotImplementedException();
        }
    }
}