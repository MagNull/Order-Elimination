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

public struct TakeDamageInfo
{
    public int RemainHealth;
    public int RemainArmor;
    public DamageCancelType CancelType;
    public IBattleObject Attacker;
    public IBattleObject Target;
}

public interface IDamageable
{
    event Action<int, int, DamageCancelType> Damaged;
    void TakeDamage(int damage, int accuracy, DamageHealTarget damageHealTarget, DamageModificator damageModificator);
}