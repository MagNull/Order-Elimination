using System.Collections.Generic;
using CharacterAbility;
using UnityEngine;

namespace OrderElimination.Battle
{
    public class SimpleDamageCalculation : IDamageCalculation
    {
        public (int healtDamage, int armorDamage, DamageCancelType damageCancelType) CalculateDamage(int damage,
            int armor, int accuracy, int evasion, DamageHealType damageHealType, List<IncomingBuff> incomingDebuffs)
        {
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

            bool hitRoll = Random.Range(0, 100) <= accuracy;
            if (!hitRoll)
                return (0, 0, DamageCancelType.Miss);
            if (damageHealType != DamageHealType.OnlyHealth)
            {
                bool evasionRoll = Random.Range(0, 100) <= evasion;
                if (evasionRoll)
                    return (0, 0, DamageCancelType.Dodge);
            }

            int armorDamage = Mathf.Clamp(damage, 0, armor);
            var healthDamage = damage - armorDamage;
            switch (damageHealType)
            {
                case DamageHealType.OnlyArmor:
                    healthDamage = 0;
                    break;
                case DamageHealType.OnlyHealth:
                    armorDamage = 0;
                    healthDamage = damage;
                    break;
            }

            return (healthDamage, armorDamage, DamageCancelType.None);
        }
    }
}