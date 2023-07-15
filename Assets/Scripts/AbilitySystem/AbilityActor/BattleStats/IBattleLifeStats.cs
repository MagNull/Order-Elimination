using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleLifeStats : IBattleStats 
    {
        public ProcessingParameter<float> MaxHealth { get; }
        public ProcessingParameter<float> MaxArmor { get; } //ValueChanged += Update Armor
        public float Health { get; set; }
        public float TotalArmor { get; set; }
        public float PureArmor { get; set; } //Value between [0 and MaxArmor]
        public float TemporaryArmor { get; } //Depletes first. Truncates at 0.
        public void AddTemporaryArmor(TemporaryArmor armor);
        public void RemoveTemporaryArmor(TemporaryArmor armor);

        public event Action<IBattleLifeStats> HealthDepleted;
        public event Action<IBattleLifeStats> LifeStatsChanged;

        public static DealtDamageInfo DistributeDamage(
            IBattleLifeStats targetStats, DamageInfo incomingDamage)
        {
            var damageRemainder = incomingDamage.DamageValue;
            var armorDamagePart = 0f;
            var healthDamagePart = 0f;
            switch (incomingDamage.DamagePriority)
            {
                case LifeStatPriority.ArmorFirst:
                    armorDamagePart = GetUnscaledStatMaxOffset(targetStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    damageRemainder -= armorDamagePart;
                    healthDamagePart = GetUnscaledStatMaxOffset(targetStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    break;
                case LifeStatPriority.HealthFirst:
                    healthDamagePart = GetUnscaledStatMaxOffset(targetStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    damageRemainder -= healthDamagePart;
                    armorDamagePart = GetUnscaledStatMaxOffset(targetStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    break;
                case LifeStatPriority.ArmorOnly:
                    armorDamagePart = GetUnscaledStatMaxOffset(targetStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    break;
                case LifeStatPriority.HealthOnly:
                    healthDamagePart = GetUnscaledStatMaxOffset(targetStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    break;
                default:
                    throw new NotImplementedException();
            }
            var totalArmorDamage = armorDamagePart * incomingDamage.ArmorMultiplier;
            var totalHealthDamage = healthDamagePart * incomingDamage.HealthMultiplier;
            return new DealtDamageInfo(incomingDamage, totalArmorDamage, totalHealthDamage);
        }

        public static DealtRecoveryInfo DistributeRecovery(
            IBattleLifeStats targetStats, RecoveryInfo incomingHeal)
        {
            var healRemainder = incomingHeal.RecoveryValue;
            var armorRecoveryPart = 0f;
            var healthRecoveryPart = 0f;
            var emptyArmor = targetStats.MaxArmor.ModifiedValue - targetStats.PureArmor;
            var emptyHealth = targetStats.MaxHealth.ModifiedValue - targetStats.Health;
            switch (incomingHeal.HealPriority)
            {
                case LifeStatPriority.ArmorFirst:
                    armorRecoveryPart = GetUnscaledStatMaxOffset(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    healRemainder -= armorRecoveryPart;
                    healthRecoveryPart = GetUnscaledStatMaxOffset(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    break;
                case LifeStatPriority.HealthFirst:
                    healthRecoveryPart = GetUnscaledStatMaxOffset(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    healRemainder -= healthRecoveryPart;
                    armorRecoveryPart = GetUnscaledStatMaxOffset(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    break;
                case LifeStatPriority.ArmorOnly:
                    armorRecoveryPart = GetUnscaledStatMaxOffset(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    break;
                case LifeStatPriority.HealthOnly:
                    healthRecoveryPart = GetUnscaledStatMaxOffset(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    break;
                default:
                    throw new NotImplementedException();
            }
            var totalArmorRecovery = armorRecoveryPart * incomingHeal.ArmorMultiplier;
            var totalHealthRecovery = healthRecoveryPart * incomingHeal.HealthMultiplier;
            return new DealtRecoveryInfo(incomingHeal, totalArmorRecovery, totalHealthRecovery);
        }

        private static float GetUnscaledStatMaxOffset(
            float maxOffsetLimit, float desiredOffset, float multiplier)
        {
            //realOffset * multiplier <= offsetLimit
            if (maxOffsetLimit < 0) throw new NotSupportedException();
            var unscaledOffsetLimit = maxOffsetLimit / multiplier;
            return MathF.Min(desiredOffset, unscaledOffsetLimit);
        }
    }
}
