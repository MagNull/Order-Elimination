using OrderElimination;
using UnityEngine;

namespace CharacterAbility
{
    public class DamageOverTimeEffect : ITickEffect
    {
        private readonly IBattleObject _target;
        private readonly DamageHealType _damageHealType;
        private readonly int _damage;
        private int _duration;

        public DamageOverTimeEffect(IBattleObject target, DamageHealType damageHealType, int damage, int duration)
        {
            _target = target;
            _damageHealType = damageHealType;
            _damage = damage;
            _duration = duration;
        }

        public void Tick(IReadOnlyBattleStats stats)
        {
            _target.TakeDamage(_damage, 100, _damageHealType);
            _duration--;
            if (_duration <= 0)
            {
                _target.RemoveTickEffect(this);
            }
        }
    }
}