using System;
using System.Collections.Generic;
using CharacterAbility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.Battle
{
    public class SimpleDamageCalculation : IDamageCalculation
    {
        public (int healthDamage, int armorDamage, DamageCancelType damageCancelType) CalculateDamage(int damage,
            DamageModificator damageModificator, int armor, int accuracy, int evasion,
            DamageHealTarget damageHealTarget, List<IncomingBuff> incomingDebuffs)
        {
            ApplyModifications(ref damage, ref accuracy, armor, damageModificator, damageHealTarget, incomingDebuffs);

            bool hitRoll = Random.Range(0, 100) <= accuracy;
            if (!hitRoll)
                return (0, 0, DamageCancelType.Miss);
            if (damageHealTarget != DamageHealTarget.OnlyHealth)
            {
                bool evasionRoll = Random.Range(0, 100) <= evasion;
                if (evasionRoll)
                    return (0, 0, DamageCancelType.Dodge);
            }

            int armorDamage = Mathf.Clamp(damage, 0, armor);
            var healthDamage = damage - armorDamage;
            switch (damageHealTarget)
            {
                case DamageHealTarget.OnlyArmor:
                    healthDamage = 0;
                    break;
                case DamageHealTarget.OnlyHealth:
                    armorDamage = 0;
                    healthDamage = damage;
                    break;
            }

            return (healthDamage, armorDamage, DamageCancelType.None);
        }

        private static void ApplyModifications(ref int damage, ref int accuracy, int armor,
            DamageModificator damageModificator, DamageHealTarget damageHealTarget, List<IncomingBuff> incomingDebuffs)
        {
            switch (damageModificator)
            {
                case DamageModificator.DoubleArmor:
                    if (armor > 0 &&
                        damageHealTarget is DamageHealTarget.Normal or DamageHealTarget.OnlyArmor)
                        damage += 2 * damage > armor
                            ? (int) Mathf.Floor(Mathf.Clamp(damage, 0, armor)) / 2
                            : damage;
                    break;
                case DamageModificator.DoubleHealth:
                    // if (damageHealTarget == DamageHealTarget.Normal && armor == 0 ||
                    //     damageHealTarget == DamageHealTarget.OnlyHealth)
                    //     damage += 2 * damage > health
                    //         ? (int) Mathf.Floor(Mathf.Clamp(damage, 0, health)) / 2
                    //         : damage;
                    break;
                case DamageModificator.Normal:
                    break;
                default:
                    throw new ArgumentException();
            }

            foreach (var incomingDebuff in incomingDebuffs)
            {
                switch (incomingDebuff.DebuffType)
                {
                    case IncomingDebuffType.Accuracy:
                        accuracy = incomingDebuff.GetModifiedValue(accuracy);
                        break;
                    case IncomingDebuffType.Attack:
                        damage = incomingDebuff.GetModifiedValue(damage);
                        break;
                }
            }
        }
    }
}