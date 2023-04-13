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
        public LifeStatsTarget HealTarget;
    }

    public class DamageInfo
    {
        public float Value;
        public float ArmorMultiplier;
        public float HealthMultiplier;
        public LifeStatsTarget DamageTarget;
        public DamageType DamageType;
        //Attacker
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
        None,
        Explosion,
        Shooting,
        Cutting
    }

    public enum LifeStatsTarget
    {
        ArmorFirst,
        HealthFirst,
        ArmorOnly,
        HealthOnly
    }
}
