using System;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using CharacterAbility.BuffEffects;
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
            
            int maximumArmorDamage = damageInfo.DamageModificator == DamageModificator.DoubleArmor ? armor / 2 : armor;
            int armorDamage = Mathf.Min(maximumArmorDamage, damageInfo.Damage);
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

            return (healthDamage,
                damageInfo.DamageModificator == DamageModificator.DoubleArmor ? armorDamage * 2 : armorDamage,
                DamageCancelType.None);
        }

        private static void ApplyModifications(ref DamageInfo damageInfo, int armor, List<IncomingBuff> incomingDebuffs)
        {
            foreach (var incomingAttackBuff in incomingDebuffs)
            {
                damageInfo = incomingAttackBuff.GetModifiedInfo(damageInfo);
            }

            if (damageInfo.Attacker is not BattleCharacter battleCharacter)
                return;

            foreach (var effect in battleCharacter.CurrentTickEffects.Where(ef => ef is OutcomingBuff))
            {
                damageInfo = ((OutcomingBuff) effect).GetModifiedInfo(damageInfo);
            }
        }
    }
}