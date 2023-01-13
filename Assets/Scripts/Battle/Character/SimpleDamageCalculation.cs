using System;
using System.Collections.Generic;
using CharacterAbility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.Battle
{
    public class SimpleDamageCalculation : IDamageCalculation
    {
        public (int healthDamage, int armorDamage, DamageCancelType cancelType) CalculateDamage(DamageInfo damageInfo,
            int armor, int evasion, List<IncomingBuff> incomingDebuffs)
        {
            ApplyModifications(ref damageInfo, armor, incomingDebuffs);

            bool hitRoll = Random.Range(0, 100) <= damageInfo.Accuracy;
            if (!hitRoll)
                return (0, 0, DamageCancelType.Miss);
            if (damageInfo.DamageHealTarget != DamageHealTarget.OnlyHealth)
            {
                bool evasionRoll = Random.Range(0, 100) <= evasion;
                if (evasionRoll)
                    return (0, 0, DamageCancelType.Dodge);
            }

            int armorDamage = Mathf.Clamp(damageInfo.Damage, 0, armor);
            var healthDamage = damageInfo.Damage - armorDamage;
            switch (damageInfo.DamageHealTarget)
            {
                case DamageHealTarget.OnlyArmor:
                    healthDamage = 0;
                    break;
                case DamageHealTarget.OnlyHealth:
                    armorDamage = 0;
                    healthDamage = damageInfo.Damage;
                    break;
            }

            return (healthDamage, armorDamage, DamageCancelType.None);
        }

        private static void ApplyModifications(ref DamageInfo damageInfo, int armor, List<IncomingBuff> incomingDebuffs)
        {
            foreach (var incomingAttackBuff in incomingDebuffs)
            {
                damageInfo = incomingAttackBuff.GetModifiedInfo(damageInfo);
            }

            switch (damageInfo.Attacker.Stats.DamageModificator)
            {
                case DamageModificator.DoubleArmor:
                    if (armor > 0 &&
                        damageInfo.DamageHealTarget is DamageHealTarget.Normal or DamageHealTarget.OnlyArmor)
                        damageInfo.Damage += 2 * damageInfo.Damage > armor
                            ? (int) Mathf.Floor(Mathf.Clamp(damageInfo.Damage, 0, armor)) / 2
                            : damageInfo.Damage;
                    break;
                case DamageModificator.DoubleHealth:
                    // if (damageInfo.DamageHealTarget == DamageHealTarget.Normal && armor == 0 ||
                    //     damageInfo.DamageHealTarget == DamageHealTarget.OnlyHealth)
                    //     damageInfo.Damage += 2 * damageInfo.Damage > health
                    //         ? (int) Mathf.Floor(Mathf.Clamp(damageInfo.Damage, 0, health)) / 2
                    //         : damageInfo.Damage;
                    break;
                case DamageModificator.Normal:
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}