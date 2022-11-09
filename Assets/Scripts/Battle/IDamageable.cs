using System;

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
    event Action<int> Damaged;
    void TakeDamage(int damage, int accuracy, DamageHealType damageHealType);
}