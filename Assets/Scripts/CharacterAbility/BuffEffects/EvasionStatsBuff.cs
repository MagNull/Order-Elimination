using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public class EvasionStatsBuff : TickEffectBase, IStatsBuffEffect
    {
        [SerializeField]
        private int _value;

        public EvasionStatsBuff(int value, int duration) : base(duration)
        {
            _value = value;
        }

        public BattleStats Apply(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Evasion = target.Stats.Evasion + _value
            };
            return newStats;
        }

        public BattleStats Remove(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Evasion = target.Stats.Evasion - _value
            };
            return newStats;
        }
    }
}