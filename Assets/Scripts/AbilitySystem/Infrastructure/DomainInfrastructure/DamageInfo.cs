using System;

namespace OrderElimination.AbilitySystem
{
    public readonly struct DamageInfo
    {
        public float DamageValue { get; }
        public float ArmorMultiplier { get; }
        public float HealthMultiplier { get; }
        public DamageType DamageType { get; }
        public LifeStatPriority DamagePriority { get; }
        public AbilitySystemActor DamageDealer { get; }

        public DamageInfo(
            float value, 
            float armorMultiplier, 
            float healthMultiplier, 
            DamageType damageType, 
            LifeStatPriority priority,
            AbilitySystemActor damageDealer)
        {
            if (value < 0) Logging.LogException( new ArgumentException("Damage value is less than 0."));
            DamageValue = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            DamageType = damageType;
            DamagePriority = priority;
            DamageDealer = damageDealer;
        }
    }

    public readonly struct DealtDamageInfo
    {
        public DamageInfo DamageInfo { get; }
        public float TotalHealthDamage { get; }
        public float TotalArmorDamage { get; }
        //Target?

        public float TotalDamage => TotalHealthDamage + TotalArmorDamage;

        public DealtDamageInfo(DamageInfo damageType, float totalArmorDamage, float totalHealthDamage)
        {
            DamageInfo = damageType;
            TotalHealthDamage = totalHealthDamage;
            TotalArmorDamage = totalArmorDamage;
        }
    }
}
