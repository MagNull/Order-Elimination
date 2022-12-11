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
        public (int healthDamage, int armorDamage, DamageCancelType damageCancelType) CalculateDamage(int damage,
            DamageModificator damageModificator,
            int armor, int accuracy, int evasion, DamageHealTarget damageHealTarget, List<IncomingBuff> incomingDebuffs);
    }
}