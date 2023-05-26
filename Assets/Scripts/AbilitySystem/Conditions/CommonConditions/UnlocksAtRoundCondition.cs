using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class UnlocksAtRoundCondition : ICommonCondition
    {
        [HideInInspector, OdinSerialize]
        private int _unlocksAtRound = 1;

        [ShowInInspector]
        public int UnlocksAtRound
        {
            get => _unlocksAtRound;
            set
            {
                if (value < 1) value = 1;
                _unlocksAtRound = value;
            }
        }

        public ICommonCondition Clone()
        {
            var clone = new UnlocksAtRoundCondition();
            clone._unlocksAtRound = _unlocksAtRound;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return battleContext.CurrentRound >= UnlocksAtRound;
        }
    }
}
