using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (value < 0) throw new ArgumentException("Heal value is less than 0.");
            Value = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            HealPriority = priority;
            Healer = healer;
        }
    }

    public readonly struct DamageInfo
    {
        public readonly float Value;
        public readonly float ArmorMultiplier;
        public readonly float HealthMultiplier;
        public readonly DamageType DamageType;
        public readonly LifeStatPriority DamagePriority;
        public readonly AbilitySystemActor DamageDealer;
        //Attacker

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
        //Attacker
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
    public enum DamageType
    {
        Melee,
        Shooting,
        Explosion,
        Magic
    }

    public enum LifeStatPriority
    {
        ArmorFirst,
        HealthFirst,
        ArmorOnly,
        HealthOnly
    }
}
