using CharacterAbility.BuffEffects;
using OrderElimination.BattleMap;

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
            var attackInfo = new DamageInfo
            {
                Attacker = new NullBattleObject(),
                Damage = _damage,
                DamageHealTarget = _damageHealTarget
            };
            tickTarget.TakeDamage(attackInfo);
            base.Tick(tickTarget);
        }
    }
}