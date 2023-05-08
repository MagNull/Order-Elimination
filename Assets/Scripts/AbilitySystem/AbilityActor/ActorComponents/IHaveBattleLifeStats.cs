using System;

namespace OrderElimination.AbilitySystem
{
    public interface IHaveBattleLifeStats
    {
        public ILifeBattleStats LifeStats { get; }

        public event Action<DealtDamageInfo> Damaged;
        public event Action<HealRecoveryInfo> Healed;

        public DealtDamageInfo TakeDamage(DamageInfo incomingDamage);

        public HealRecoveryInfo TakeHeal(HealInfo incomingHeal);
    }

    public static class IHaveLifeStatsExtensions
    {
        [Obsolete("Use " + nameof(IHaveBattleLifeStats.TakeDamage) + " instead.")]
        public static DealtDamageInfo NoEventTakeDamage(this IHaveBattleLifeStats target, DamageInfo incomingDamage)
        {
            var damageRemainder = incomingDamage.Value;
            var armorDamage = 0f;
            var healthDamage = 0f;
            switch (incomingDamage.DamagePriority)
            {
                case LifeStatPriority.ArmorFirst:
                    armorDamage = GetUnscaledStatMaxValue(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    damageRemainder -= armorDamage;
                    healthDamage = GetUnscaledStatMaxValue(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    break;
                case LifeStatPriority.HealthFirst:
                    healthDamage = GetUnscaledStatMaxValue(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    damageRemainder -= healthDamage;
                    armorDamage = GetUnscaledStatMaxValue(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    break;
                case LifeStatPriority.ArmorOnly:
                    armorDamage = GetUnscaledStatMaxValue(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    break;
                case LifeStatPriority.HealthOnly:
                    healthDamage = GetUnscaledStatMaxValue(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    break;
                default:
                    throw new NotImplementedException();
            }
            target.LifeStats.TotalArmor -= armorDamage * incomingDamage.ArmorMultiplier;
            target.LifeStats.Health -= healthDamage * incomingDamage.HealthMultiplier;
            return new DealtDamageInfo(healthDamage, armorDamage, incomingDamage.DamageType, incomingDamage.DamageDealer);
        }

        [Obsolete("Use " + nameof(IHaveBattleLifeStats.TakeHeal) + " instead.")]
        public static HealRecoveryInfo NoEventTakeHeal(this IHaveBattleLifeStats target, HealInfo incomingHeal)
        {
            var healRemainder = incomingHeal.Value;
            var armorRecovery = 0f;
            var healthRecovery = 0f;
            var emptyArmor = target.LifeStats.MaxArmor.ModifiedValue - target.LifeStats.PureArmor;
            var emptyHealth = target.LifeStats.MaxHealth.ModifiedValue - target.LifeStats.Health;
            switch (incomingHeal.HealPriority)
            {
                case LifeStatPriority.ArmorFirst:
                    armorRecovery = GetUnscaledStatMaxValue(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    healRemainder -= armorRecovery;
                    healthRecovery = GetUnscaledStatMaxValue(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    break;
                case LifeStatPriority.HealthFirst:
                    healthRecovery = GetUnscaledStatMaxValue(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    healRemainder -= healthRecovery;
                    armorRecovery = GetUnscaledStatMaxValue(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    break;
                case LifeStatPriority.ArmorOnly:
                    armorRecovery = GetUnscaledStatMaxValue(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    break;
                case LifeStatPriority.HealthOnly:
                    healthRecovery = GetUnscaledStatMaxValue(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    break;
                default:
                    throw new NotImplementedException();
            }
            target.LifeStats.PureArmor += armorRecovery * incomingHeal.ArmorMultiplier;
            target.LifeStats.Health += healthRecovery * incomingHeal.HealthMultiplier;
            return new HealRecoveryInfo(healthRecovery, armorRecovery, incomingHeal.Healer);
        }

        private static float GetUnscaledStatMaxValue(float limitingValue, float damage, float multiplier)
        {
            var unscaledLimitedValue = limitingValue / multiplier;
            return MathF.Min(damage, unscaledLimitedValue);
        }
    }
}
