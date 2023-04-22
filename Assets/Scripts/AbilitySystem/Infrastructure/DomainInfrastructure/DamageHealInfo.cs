using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class HealInfo
    {
        public float Value;
        public float ArmorMultiplier;
        public float HealthMultiplier;
        public DamagePriority HealTarget;
    }

    public readonly struct DamageInfo
    {
        public readonly float Value;
        public readonly float ArmorMultiplier;
        public readonly float HealthMultiplier;
        public readonly DamageType DamageType;
        public readonly DamagePriority DamagePriority;
        //Attacker

        public DamageInfo(float value, float armorMultiplier, float healthMultiplier, DamageType damageType, DamagePriority priority)
        {
            Value = value;
            ArmorMultiplier = armorMultiplier;
            HealthMultiplier = healthMultiplier;
            DamageType = damageType;
            DamagePriority = priority;
        }
    }

    public readonly struct DealtDamageInfo
    {
        public readonly float HealthDamage;
        public readonly float ArmorDamage;
        public readonly DamageType DamageType;
        //Attacker
        //Target

        public DealtDamageInfo(float healthDamage, float armorDamage, DamageType damageType)
        {
            HealthDamage = healthDamage;
            ArmorDamage = armorDamage;
            DamageType = damageType;
        }
    }

    public readonly struct HealRecoveryInfo
    {
        public readonly float RecoveredHealth;
        public readonly float RecoveredArmor;

        public HealRecoveryInfo(float recoveredHealth, float recoveredArmor)
        {
            RecoveredHealth = recoveredHealth;
            RecoveredArmor = recoveredArmor;
        }
    }
    public enum DamageType
    {
        Melee,
        Shooting,
        Explosion,
        Magic
    }

    public enum DamagePriority
    {
        ArmorFirst,
        HealthFirst,
        ArmorOnly,
        HealthOnly
    }
}
