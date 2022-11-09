﻿using UnityEngine;

namespace OrderElimination.Battle
{
    public class SimpleDamageCalculation : IDamageCalculation
    {
        public (int healtDamage, int armorDamage) CalculateDamage(int attack, int armor, int accuracy, int evasion,
            DamageHealType damageHealType)
        {
            bool hitRoll = Random.Range(0, 100) <= accuracy;
            if (!hitRoll)
                return (0, 0);
            bool evasionRoll = Random.Range(0, 100) <= evasion;
            if (evasionRoll)
                return (0, 0);
            int armorDamage =  Mathf.Clamp(attack, 0, armor);
            var healthDamage = attack - armorDamage;
            switch (damageHealType)
            {
                case DamageHealType.OnlyArmor:
                    healthDamage = 0;
                    break;
                case DamageHealType.OnlyHealth:
                    armorDamage = 0;
                    healthDamage = attack;
                    break;
            }
            return (healthDamage, armorDamage);
        }
    }
}