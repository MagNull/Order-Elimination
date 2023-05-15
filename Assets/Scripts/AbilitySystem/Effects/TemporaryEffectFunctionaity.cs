using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TemporaryEffectFunctionaity
    {
        [HideInInspector, OdinSerialize]
        private int _applyingDuration = 1;

        [PropertyOrder(-1), SuffixLabel("rounds", overlay: true)]
        [ShowInInspector]
        public int ApplyingDuration
        {
            get => _applyingDuration;
            set
            {
                if (value < 1) value = 1;
                _applyingDuration = value;
            }
        }

        [PropertyOrder(1)]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _onTimeOutInstruction;

        public virtual void OnTimeOut(BattleEffect effect)
        {
            Debug.Log($"Effect {effect.EffectData.View.Name} has expired.");
            _onTimeOutInstruction?.Execute(effect);
        }
    }
}
