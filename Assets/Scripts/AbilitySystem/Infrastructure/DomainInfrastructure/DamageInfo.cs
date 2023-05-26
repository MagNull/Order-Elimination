using System;

namespace OrderElimination.AbilitySystem
{
    public readonly struct DamageInfo
    {
        public readonly float Value;
        public readonly float ArmorMultiplier;
        public readonly float HealthMultiplier;
        public readonly DamageType DamageType;
        public readonly LifeStatPriority DamagePriority;
        public readonly AbilitySystemActor DamageDealer;

        public DamageInfo(
            float value, 
            float armorMultiplier, 
            float healthMultiplier, 
            DamageType damageType, 
            LifeStatPriority priority,
            AbilitySystemActor damageDealer)
        {
            if (value < 0) throw new ArgumentException("Damage value is less than 0.");
            Value = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            DamageType = damageType;
            DamagePriority = priority;
            DamageDealer = damageDealer;
        }
    }

    public readonly struct DealtDamageInfo
    {
        public readonly float HealthDamage;
        public readonly float ArmorDamage;
        public readonly DamageType DamageType;
        public readonly AbilitySystemActor DamageDealer;
        //Target

        public float TotalDamage => HealthDamage + ArmorDamage;

        public DealtDamageInfo(float healthDamage, float armorDamage, DamageType damageType, AbilitySystemActor damageDealer)
        {
            HealthDamage = healthDamage;
            ArmorDamage = armorDamage;
            DamageType = damageType;
            DamageDealer = damageDealer;
        }
    }

    public enum DamageType
    {
        Melee,
        Shooting,
        Explosion,
        Magic,
        //Effect (Bleeding)
    }
}
