using System.Collections.Generic;
using CharacterAbility;

namespace OrderElimination.Battle
{
    public enum DamageCancelType
    {
        None,
        Miss,
        Dodge
    }

    public interface IDamageCalculation
    {
        public (int healtDamage, int armorDamage, DamageCancelType damageCancelType) CalculateDamage(int damage,
            int armor, int accuracy, int evasion, DamageHealType damageHealType, List<IncomingBuff> incomingDebuffs);
    }
}