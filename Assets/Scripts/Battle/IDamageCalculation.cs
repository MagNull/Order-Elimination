using System.Collections.Generic;
using CharacterAbility;

namespace OrderElimination.Battle
{
    public interface IDamageCalculation
    {
        public (int healtDamage, int armorDamage) CalculateDamage(int damage, int armor, int accuracy, int evasion,
            DamageHealType damageHealType, List<IncomingDebuff> incomingDebuffs);
    }
}