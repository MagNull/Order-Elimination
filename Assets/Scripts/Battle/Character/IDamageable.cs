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

public enum DamageType
{
    None,
    Explosion,
    Shooting,
    Cutting
}

public struct DamageInfo
{
    public float Damage;
    public float ArmorMultiplier;
    public float HealthMultiplier;
    public int Accuracy;
    public DamageType DamageType;
    public IBattleObject Attacker;
    public IBattleObject Target;
    public DamageHealTarget DamageHealTarget;
}
public struct TakeDamageInfo
{
    public int HealthDamage;
    public int ArmorDamage;
    public DamageCancelType CancelType;
    public IBattleObject Attacker;
}

public interface IDamageable
{
    event Action<TakeDamageInfo> Damaged;
    TakeDamageInfo TakeDamage(DamageInfo damageInfo);
}