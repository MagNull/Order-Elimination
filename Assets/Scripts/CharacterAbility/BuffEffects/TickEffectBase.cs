using System;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public abstract class TickEffectBase : ITickEffect
    {
        [SerializeField]
        private int _duration;

        protected TickEffectBase(int duration)
        {
            _duration = duration;
        }

        public virtual void Tick(IBattleObject target)
        {
            _duration--;
            if (_duration <= 0)
                target.RemoveTickEffect(this);
        }
    }
}