using System;

namespace OrderElimination.AbilitySystem
{
    public readonly struct HealInfo
    {
        public readonly float Value;
        public readonly float ArmorMultiplier;
        public readonly float HealthMultiplier;
        public readonly LifeStatPriority HealPriority;
        public readonly AbilitySystemActor Healer;

        public HealInfo(
            float value, 
            float armorMultiplier, 
            float healthMultiplier, 
            LifeStatPriority priority, 
            AbilitySystemActor healer)
        {
            if (value < 0) Logging.LogException( new ArgumentException("Heal value is less than 0."));
            Value = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            HealPriority = priority;
            Healer = healer;
        }
    }

    public readonly struct HealRecoveryInfo
    {
        public readonly float RecoveredHealth;
        public readonly float RecoveredArmor;
        public readonly AbilitySystemActor Healer;

        public HealRecoveryInfo(float recoveredHealth, float recoveredArmor, AbilitySystemActor healer)
        {
            RecoveredHealth = recoveredHealth;
            RecoveredArmor = recoveredArmor;
            Healer = healer;
        }
    }
}
