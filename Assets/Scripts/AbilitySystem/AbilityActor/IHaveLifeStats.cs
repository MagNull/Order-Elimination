using System;

namespace OrderElimination.AbilitySystem
{
    public interface IHaveLifeStats
    {
        public ILifeStats LifeStats { get; }

        public event Action<DealtDamageInfo> Damaged;
        public event Action<HealRecoveryInfo> Healed;

        public DealtDamageInfo TakeDamage(DamageInfo incomingDamage);

        public HealRecoveryInfo TakeHeal(HealInfo incomingHeal);
    }

    public static class IHaveLifeStatsExtensions
    {
        [Obsolete("Use " + nameof(IHaveLifeStats.TakeDamage) + " instead.")]
        public static DealtDamageInfo NoEventTakeDamage(this IHaveLifeStats target, DamageInfo incomingDamage)
        {
            var damageRemainder = incomingDamage.Value;
            var armorDamage = 0f;
            var healthDamage = 0f;
            switch (incomingDamage.DamageTarget)
            {
                case LifeStatsTarget.ArmorFirst:
                    armorDamage = GetUnscaledStatMaxValue(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    target.LifeStats.TotalArmor -= armorDamage * incomingDamage.ArmorMultiplier;
                    damageRemainder -= armorDamage;

                    healthDamage = GetUnscaledStatMaxValue(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    target.LifeStats.Health -= healthDamage;
                    break;
                case LifeStatsTarget.HealthFirst:
                    healthDamage = GetUnscaledStatMaxValue(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    target.LifeStats.Health -= healthDamage * incomingDamage.HealthMultiplier;
                    damageRemainder -= healthDamage;

                    armorDamage = GetUnscaledStatMaxValue(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    target.LifeStats.Health -= armorDamage;
                    break;
                case LifeStatsTarget.ArmorOnly:
                    armorDamage = GetUnscaledStatMaxValue(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    target.LifeStats.Health -= armorDamage;
                    break;
                case LifeStatsTarget.HealthOnly:
                    healthDamage = GetUnscaledStatMaxValue(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    target.LifeStats.Health -= healthDamage;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new DealtDamageInfo(healthDamage, armorDamage, incomingDamage.DamageType);
        }

        [Obsolete("Use " + nameof(IHaveLifeStats.TakeHeal) + " instead.")]
        public static HealRecoveryInfo NoEventTakeHeal(this IHaveLifeStats target, HealInfo incomingHeal)
        {
            var healRemainder = incomingHeal.Value;
            var armorRecovery = 0f;
            var healthRecovery = 0f;
            var emptyArmor = target.LifeStats.MaxArmor.ModifiedValue - target.LifeStats.PureArmor;
            var emptyHealth = target.LifeStats.MaxHealth.ModifiedValue - target.LifeStats.Health;
            switch (incomingHeal.HealTarget)
            {
                case LifeStatsTarget.ArmorFirst:
                    armorRecovery = GetUnscaledStatMaxValue(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    target.LifeStats.PureArmor += armorRecovery * incomingHeal.ArmorMultiplier;
                    healRemainder -= armorRecovery;

                    healthRecovery = GetUnscaledStatMaxValue(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    target.LifeStats.Health += healthRecovery;
                    break;
                case LifeStatsTarget.HealthFirst:
                    healthRecovery = GetUnscaledStatMaxValue(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    target.LifeStats.Health += healthRecovery * incomingHeal.HealthMultiplier;
                    healRemainder -= healthRecovery;

                    armorRecovery = GetUnscaledStatMaxValue(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    target.LifeStats.Health += armorRecovery;
                    break;
                case LifeStatsTarget.ArmorOnly:
                    armorRecovery = GetUnscaledStatMaxValue(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    target.LifeStats.Health += armorRecovery;
                    break;
                case LifeStatsTarget.HealthOnly:
                    healthRecovery = GetUnscaledStatMaxValue(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    target.LifeStats.Health += healthRecovery;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new HealRecoveryInfo(healthRecovery, armorRecovery);
        }

        public static void OnMaxArmorChanged(this IHaveLifeStats target)
        {
            var maxArmor = target.LifeStats.MaxArmor.ModifiedValue;
            if (maxArmor < target.LifeStats.PureArmor)
                target.LifeStats.PureArmor = maxArmor;
        }

        public static void OnMaxHealthChanged(this IHaveLifeStats target)
        {
            var maxHealth = target.LifeStats.MaxHealth.ModifiedValue;
            if (maxHealth < target.LifeStats.Health)
                target.LifeStats.Health = maxHealth;
        }

        private static float GetUnscaledStatMaxValue(float limitingValue, float damage, float multiplier)
        {
            var unscaledLimitedValue = limitingValue / multiplier;
            return MathF.Min(damage, unscaledLimitedValue);
        }
    }
}
