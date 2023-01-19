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

            Debug.Log(damageInfo.Accuracy);
            bool hitRoll = Random.Range(0, 100) <= damageInfo.Accuracy;
            if (!hitRoll)
                return (0, 0, DamageCancelType.Miss);
            if (damageInfo.DamageHealTarget != DamageHealTarget.OnlyHealth)
            {
                bool evasionRoll = Random.Range(0, 100) <= evasion;
                if (evasionRoll)
                    return (0, 0, DamageCancelType.Dodge);
            }

            var incomingArmorDamage = damageInfo.DamageModificator == DamageModificator.DoubleArmor 
                ? 2 * damageInfo.Damage
                : damageInfo.Damage;
            var clampedArmorDamage = Mathf.Clamp(incomingArmorDamage, 0, armor);
            var healthDamage = (incomingArmorDamage - clampedArmorDamage) / 2;
            healthDamage = damageInfo.DamageModificator == DamageModificator.DoubleHealth
                ? 2 * healthDamage
                : healthDamage;
            switch (damageInfo.DamageHealTarget)
            {
                case DamageHealTarget.OnlyArmor:
                    healthDamage = 0;
                    break;
                case DamageHealTarget.OnlyHealth:
                    clampedArmorDamage = 0;
                    healthDamage = damageInfo.Damage;
                    break;
            }

            return (healthDamage, clampedArmorDamage, DamageCancelType.None);
        }

        private static void ApplyModifications(ref DamageInfo damageInfo, int armor, List<IncomingBuff> incomingDebuffs)
        {
            foreach (var incomingAttackBuff in incomingDebuffs)
            {
                damageInfo = incomingAttackBuff.GetModifiedInfo(damageInfo);
            }
        }
    }
}