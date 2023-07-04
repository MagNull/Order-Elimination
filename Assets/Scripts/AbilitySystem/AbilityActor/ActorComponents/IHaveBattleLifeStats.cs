using System;
using static UnityEngine.GraphicsBuffer;

namespace OrderElimination.AbilitySystem
{
    public interface IHaveBattleLifeStats
    {
        public IBattleLifeStats LifeStats { get; }

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
                    armorDamage = GetUnscaledStatMaxOffset(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    damageRemainder -= armorDamage;
                    healthDamage = GetUnscaledStatMaxOffset(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    break;
                case LifeStatPriority.HealthFirst:
                    healthDamage = GetUnscaledStatMaxOffset(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
                    damageRemainder -= healthDamage;
                    armorDamage = GetUnscaledStatMaxOffset(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    break;
                case LifeStatPriority.ArmorOnly:
                    armorDamage = GetUnscaledStatMaxOffset(target.LifeStats.TotalArmor, damageRemainder, incomingDamage.ArmorMultiplier);
                    break;
                case LifeStatPriority.HealthOnly:
                    healthDamage = GetUnscaledStatMaxOffset(target.LifeStats.Health, damageRemainder, incomingDamage.HealthMultiplier);
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
                    armorRecovery = GetUnscaledStatMaxOffset(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    healRemainder -= armorRecovery;
                    healthRecovery = GetUnscaledStatMaxOffset(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    break;
                case LifeStatPriority.HealthFirst:
                    healthRecovery = GetUnscaledStatMaxOffset(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    healRemainder -= healthRecovery;
                    armorRecovery = GetUnscaledStatMaxOffset(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    break;
                case LifeStatPriority.ArmorOnly:
                    armorRecovery = GetUnscaledStatMaxOffset(emptyArmor, healRemainder, incomingHeal.ArmorMultiplier);
                    break;
                case LifeStatPriority.HealthOnly:
                    healthRecovery = GetUnscaledStatMaxOffset(emptyHealth, healRemainder, incomingHeal.HealthMultiplier);
                    break;
                default:
                    throw new NotImplementedException();
            }
            target.LifeStats.PureArmor += armorRecovery * incomingHeal.ArmorMultiplier;
            target.LifeStats.Health += healthRecovery * incomingHeal.HealthMultiplier;
            return new HealRecoveryInfo(healthRecovery, armorRecovery, incomingHeal.Healer);
        }

        private static float GetUnscaledStatMaxOffset(
            float maxOffsetLimit, float desiredOffset, float multiplier)
        {
            //realOffset * multiplier <= offsetLimit
            if (maxOffsetLimit < 0) throw new NotSupportedException();
            var unscaledOffsetLimit = maxOffsetLimit / multiplier;
            return MathF.Min(desiredOffset, unscaledOffsetLimit);
        }

        //private static (float armorOffset, float healthOffset) CalculateStatOffset(
        //    float totalOffset, 
        //    LifeStatPriority statPriority,
        //    float armorMultiplier,
        //    float healthMultiplier,
        //    float maxArmorOffset,
        //    float maxHealthOffset)
        //{
        //    var deltaRemainder = MathF.Abs(totalOffset);
        //    var armorOffset = 0f;
        //    var healthOffset = 0f;
        //    switch (statPriority)
        //    {
        //        case LifeStatPriority.ArmorFirst:
        //            armorOffset = GetUnscaledStatMaxOffset(maxArmorOffset, deltaRemainder, armorMultiplier);
        //            deltaRemainder -= armorOffset;
        //            healthOffset = GetUnscaledStatMaxOffset(maxHealthOffset, deltaRemainder, healthMultiplier);
        //            break;
        //        case LifeStatPriority.HealthFirst:
        //            healthOffset = GetUnscaledStatMaxOffset(maxHealthOffset, deltaRemainder, healthMultiplier);
        //            deltaRemainder -= healthOffset;
        //            armorOffset = GetUnscaledStatMaxOffset(maxArmorOffset, deltaRemainder, armorMultiplier);
        //            break;
        //        case LifeStatPriority.ArmorOnly:
        //            armorOffset = GetUnscaledStatMaxOffset(maxArmorOffset, deltaRemainder, armorMultiplier);
        //            break;
        //        case LifeStatPriority.HealthOnly:
        //            healthOffset = GetUnscaledStatMaxOffset(maxHealthOffset, deltaRemainder, healthMultiplier);
        //            break;
        //        default:
        //            throw new NotImplementedException();
        //    }
        //    return new(armorOffset * armorMultiplier, healthOffset * healthMultiplier);
        //}
    }
}
