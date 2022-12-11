using CharacterAbility.BuffEffects;

namespace CharacterAbility
{
    public class Damage : TickEffectBase
    {
        private readonly DamageHealType _damageHealType;
        private readonly int _damage;

        public Damage(DamageHealType damageHealType, int damage, int duration) : base(duration)
        {
            _damageHealType = damageHealType;
            _damage = damage;
        }

        public override void Tick(IBattleObject tickTarget)
        {
            tickTarget.TakeDamage(_damage, 100, _damageHealType);
            base.Tick(tickTarget);
        }
    }
}