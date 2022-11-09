using System;

public enum DamageHealType
{
    Normal,
    OnlyHealth,
    OnlyArmor
}

public interface IDamageable
{
    event Action<int> Damaged;
    void TakeDamage(int damage, int accuracy, DamageHealType damageHealType);
}