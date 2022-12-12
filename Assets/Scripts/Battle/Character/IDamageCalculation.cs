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
        public (int healthDamage, int armorDamage, DamageCancelType cancelType) CalculateDamage(DamageInfo damageInfo,
            int armor, int evasion, List<IncomingBuff> incomingDebuffs);
    }
}