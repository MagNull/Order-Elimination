using System;
using OrderElimination.Battle;

public enum DamageModificator
{
    Normal,
    DoubleArmor,
    DoubleHealth
}

public enum DamageHealTarget
{
    Normal,
    OnlyHealth,
    OnlyArmor,
}

public interface IDamageable
{
    event Action<int, int, DamageCancelType> Damaged;
    void TakeDamage(int damage, int accuracy, DamageHealTarget damageHealTarget, DamageModificator damageModificator);
}