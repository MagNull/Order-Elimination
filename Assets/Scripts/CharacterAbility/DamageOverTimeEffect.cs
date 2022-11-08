using OrderElimination;
using UnityEngine;

namespace CharacterAbility
{
    public class DamageOverTimeEffect : ITickEffect
    {
        private readonly IBattleObject _target;
        private readonly int _damage;
        private int _duration;

        public DamageOverTimeEffect(IBattleObject target, int damage, int duration)
        {
            _target = target;
            _damage = damage;
            _duration = duration;
        }

        public void Tick(IReadOnlyBattleStats stats)
        {
            _target.TakeDamage(_damage, 100);
            _duration--;
            if (_duration <= 0)
            {
                _target.RemoveTickEffect(this);
            }
        }
    }
}