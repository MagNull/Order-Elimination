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

        public AttackStatsBuff(int value, int duration) : base(duration)
        {
            _value = value;
        }

        public BattleStats Apply(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Attack = target.Stats.Attack + _value
            };
            Debug.Log("AttackStatsBuff Apply " + target.Stats.Attack + " " + _value + "Result: " + newStats.Attack);
            return newStats;
        }

        public BattleStats Remove(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Attack = target.Stats.Attack - _value
            };
            return newStats;
        }
    }
}