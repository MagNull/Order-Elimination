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

        [HideInInspector, OdinSerialize]
        private IEffectInstruction _onTimeOutInstruction;

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
        [ShowInInspector]
        public IEffectInstruction OnTimeOutInstruction
        {
            get => _onTimeOutInstruction;
            set => _onTimeOutInstruction = value;
        }


        public virtual void OnTimeOut(BattleEffect effect)
        {
            Logging.Log($"Effect {effect.EffectData.View.Name} has expired.");
            _onTimeOutInstruction?.Execute(effect);
        }
    }
}
