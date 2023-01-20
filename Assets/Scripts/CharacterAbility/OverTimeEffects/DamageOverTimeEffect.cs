using CharacterAbility.BuffEffects;
using OrderElimination.BM;

namespace CharacterAbility
{
    public class DamageOverTimeEffect : TickEffectBase
    {
        public DamageHealTarget DamageHealTarget;
        public int Damage { get; }

        public DamageOverTimeEffect(
            DamageHealTarget damageHealTarget, int damage, int duration, ITickEffectView view) : base(duration, view )
        {
            DamageHealTarget = damageHealTarget;
            Damage = damage;
        }

        public override void Tick(ITickTarget tickTarget)
        {
            var attackInfo = new DamageInfo
            {
                Attacker = new NullBattleObject(),
                Damage = Damage,
                Accuracy = 100,
                DamageHealTarget = DamageHealTarget
            };
            tickTarget.TakeDamage(attackInfo);
            base.Tick(tickTarget);
        }
    }
}