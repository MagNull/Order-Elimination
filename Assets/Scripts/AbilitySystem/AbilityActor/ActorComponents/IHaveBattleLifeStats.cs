using System;

namespace OrderElimination.AbilitySystem
{
    public interface IHaveBattleLifeStats
    {
        public IBattleLifeStats BattleStats { get; }

        public event Action<DealtDamageInfo> Damaged;
        public event Action<DealtRecoveryInfo> Healed;

        public DealtDamageInfo TakeDamage(DamageInfo incomingDamage);

        public DealtRecoveryInfo TakeRecovery(RecoveryInfo incomingHeal);
    }
}
