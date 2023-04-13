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
        public (float healthDamage, float armorDamage, DamageCancelType cancelType) CalculateDamage(DamageInfo damageInfo,
            float armor, float evasion, List<IncomingBuff> incomingDebuffs);
    }
}