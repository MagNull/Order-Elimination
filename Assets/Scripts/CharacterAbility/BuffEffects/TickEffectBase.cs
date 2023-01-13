using System;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public abstract class TickEffectBase : ITickEffect
    {
        [SerializeField]
        private int _duration;

        private ITickEffectView _effectView;

        public ITickEffectView GetEffectView() => _effectView;

        protected TickEffectBase(int duration, ITickEffectView effectView)
        {
            _duration = duration;
            _effectView = effectView;
        }

        public virtual void Tick(ITickTarget target)
        {
            _duration--;
            if (_duration <= 0)
                target.RemoveTickEffect(this);
        }
    }
}