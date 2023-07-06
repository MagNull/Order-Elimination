using System;

namespace OrderElimination.AbilitySystem
{
    public readonly struct RecoveryInfo
    {
        public float RecoveryValue { get; }
        public float ArmorMultiplier { get; }
        public float HealthMultiplier { get; }
        public LifeStatPriority HealPriority { get; }
        public AbilitySystemActor Healer { get; }

        public RecoveryInfo(
            float value, 
            float armorMultiplier, 
            float healthMultiplier, 
            LifeStatPriority priority, 
            AbilitySystemActor healer)
        {
            if (value < 0) Logging.LogException( new ArgumentException("Heal value is less than 0."));
            RecoveryValue = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            HealPriority = priority;
            Healer = healer;
        }
    }

    public readonly struct DealtRecoveryInfo
    {
        public RecoveryInfo RecoveryInfo { get; }
        public float TotalHealthRecovery { get; }
        public float TotalArmorRecovery { get; }

        public float TotalRecovery => TotalHealthRecovery + TotalArmorRecovery;

        public DealtRecoveryInfo(
            RecoveryInfo recoveryInfo, float totalArmorRecovery, float totalHealthRecovery)
        {
            RecoveryInfo = recoveryInfo;
            TotalHealthRecovery = totalHealthRecovery;
            TotalArmorRecovery = totalArmorRecovery;
        }
    }
}
