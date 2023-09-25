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
        public bool IsFromEffect { get; }

        public DamageInfo(
            float value, 
            float armorMultiplier, 
            float healthMultiplier, 
            DamageType damageType, 
            LifeStatPriority priority,
            AbilitySystemActor damageDealer,
            bool fromEffect)
        {
            if (value < 0) Logging.LogException( new ArgumentException("Damage value is less than 0."));
            DamageValue = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            DamageType = damageType;
            DamagePriority = priority;
            DamageDealer = damageDealer;
            IsFromEffect = fromEffect;
        }
    }

    public readonly struct DealtDamageInfo
    {
        public DamageInfo IncomingDamage { get; }
        public AbilitySystemActor DamageDealer => IncomingDamage.DamageDealer;
        public AbilitySystemActor DamageTarget { get; }
        public float DealtDamageToHealth { get; }
        public float DealtDamageToArmor { get; }
        public float TotalDealtDamage => DealtDamageToHealth + DealtDamageToArmor;

        public DealtDamageInfo(
            AbilitySystemActor damageTarget, 
            DamageInfo incomingDamage, 
            float totalArmorDamage, float totalHealthDamage)
        {
            DamageTarget = damageTarget;
            IncomingDamage = incomingDamage;
            DealtDamageToHealth = totalHealthDamage;
            DealtDamageToArmor = totalArmorDamage;
        }
    }
}
