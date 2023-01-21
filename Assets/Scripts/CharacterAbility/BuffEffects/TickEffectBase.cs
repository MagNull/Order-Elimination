using System;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    [Serializable]
    public abstract class TickEffectBase : ITickEffect
    {
        [SerializeField]
        private int _duration;
        public int Duration => _duration;

        [SerializeField]
        private ITickEffectView _effectView;
        private readonly int _startDuration;

        public ITickEffectView GetEffectView() => _effectView;

        protected TickEffectBase(int duration, ITickEffectView effectView)
        {
            _duration = duration;
            _startDuration = duration;
            _effectView = effectView;
        }

        public virtual void Tick(ITickTarget target)
        {
            if (_duration == _startDuration)
                OnStartTick(target);
            _duration--;
            OnTicked(target);
            if (_duration <= 0)
            {
                OnEndTick(target);
                target.RemoveTickEffect(this);
            }
        }

        public void RemoveTickEffect(ITickTarget target)
        {
            OnEndTick(target);
            target.RemoveTickEffect(this);
        }

        //TODO: Think about it

        protected virtual void OnEndTick(ITickTarget target)
        {
        }

        protected virtual void OnStartTick(ITickTarget target)
        {
        }

        protected virtual void OnTicked(ITickTarget target)
        {
        }
    }
}