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
    event Action<int, int> Damaged;
    event Action Died;
    void TakeDamage(int damage, int accuracy, DamageHealType damageHealType);
}