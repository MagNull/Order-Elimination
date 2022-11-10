using System;
using OrderElimination.Battle;

//TODO: Refactor Double

public enum AttackType
{
    Normal,
    DoubleArmor,
    DoubleHealth
}
public enum DamageHealType
{
    Normal,
    OnlyHealth,
    OnlyArmor,
}

public interface IDamageable
{
    event Action<int, int, DamageCancelType> Damaged;
    void TakeDamage(int damage, int accuracy, DamageHealType damageHealType);
}