using CharacterAbility.BuffEffects;
using OrderElimination.BM;
using UnityEngine;

namespace CharacterAbility
{
    public class DamageOverTimeEffect : TickEffectBase
    {
        [SerializeField]
        private int _damage;
        [SerializeField]
        private DamageHealTarget _damageHealTarget;

        public int Damage => _damage;

        public DamageHealTarget DamageHealTarget => _damageHealTarget;

        public DamageOverTimeEffect(
            DamageHealTarget damageHealTarget, int damage, int duration, bool isUnique, ITickEffectView view) : base(
            duration, view, isUnique)
        {
            _damageHealTarget = damageHealTarget;
            _damage = damage;
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

        public override bool Equals(ITickEffect tickEffect)
        {
            return tickEffect is DamageOverTimeEffect damageOverTimeEffect &&
                   DamageHealTarget == damageOverTimeEffect.DamageHealTarget &&
                   Damage == damageOverTimeEffect.Damage;
        }
    }
}