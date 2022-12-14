﻿using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public class AccuracyStatsBuff : TickEffectBase, IStatsBuffEffect
    {
        [SerializeField]
        private int _value;

        public AccuracyStatsBuff(int value, int duration) : base(duration)
        {
            _value = value;
        }
        
        public BattleStats Apply(IBattleObject target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Accuracy = target.Stats.Accuracy + _value
            };
            return newStats;
        }

        public BattleStats Remove(IBattleObject target)
        {
            var newStats = new BattleStats(target.Stats)
            {
                Accuracy = target.Stats.Accuracy - _value
            };
            return newStats;
        }
    }
}