using CharacterAbility.BuffEffects;

namespace CharacterAbility
{
    public class DamageOverTimeEffect : TickEffectBase
    {
        private readonly DamageHealTarget _damageHealTarget;
        private readonly int _damage;

        public DamageOverTimeEffect(DamageHealTarget damageHealTarget, int damage, int duration) : base(duration)
        {
            _damageHealTarget = damageHealTarget;
            _damage = damage;
        }

        public override void Tick(ITickTarget tickTarget)
        {
            tickTarget.TakeDamage(_damage, 100, _damageHealTarget, DamageModificator.Normal);
            base.Tick(tickTarget);
        }
    }
}