using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public class AttackStatsBuff : TickEffectBase, IStatsBuffEffect
    {
        [SerializeField]
        private int _value;
        private int _attackOld;

        public AttackStatsBuff(int value, int duration, ITickEffectView view) : base(duration, view)
        {
            _value = value;
        }

        public BattleStats Apply(IBuffTarget target)
        {
            _attackOld = target.Stats.Attack;
            var newStats = new BattleStats(target.Stats)
            {
                Attack = (int)(target.Stats.Attack * (1f + _value / 100f)) 
            };
            return newStats;
        }

        public BattleStats Remove(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Attack = _attackOld
            };
            return newStats;
        }
    }
}