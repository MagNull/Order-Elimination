using System;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using CharacterAbility.BuffEffects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.Battle
{
    //TODO refactor (move to HitCalculation and TakeDamage)
    public class SimpleDamageCalculation : IDamageCalculation
    {
        public (float healthDamage, float armorDamage, DamageCancelType cancelType) CalculateDamage(DamageInfo damageInfo,
            float armor, float evasion, List<IncomingBuff> incomingDebuffs)
        {
            ApplyModifications(ref damageInfo, armor, incomingDebuffs);

            var hitRoll = Random.Range(0, 100) <= damageInfo.Accuracy;
            if (!hitRoll)
                return (0, 0, DamageCancelType.Miss);
            if (damageInfo.DamageHealTarget != DamageHealTarget.OnlyHealth)
            {
                var evasionRoll = Random.Range(0, 100) <= evasion;
                if (evasionRoll)
                    return (0, 0, DamageCancelType.Dodge);
            }
            
            float maximumArmorDamage = armor / damageInfo.ArmorMultiplier;
            float armorDamage = Mathf.Min(maximumArmorDamage, damageInfo.Damage);
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
                armorDamage * damageInfo.ArmorMultiplier,
                DamageCancelType.None);
        }

        private static void ApplyModifications(ref DamageInfo damageInfo, float armor, List<IncomingBuff> incomingDebuffs)
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