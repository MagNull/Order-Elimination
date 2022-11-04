using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BattleStats
{
    // Заглушка
    private int _hp;
    private int _attack;
    private int _armor;
    private int _evasion;
    private int _accuracy;
    private int _movement;

    public int GetHp() => _hp;
    public int GetAttack() => _attack;
    public int GetArmor() => _armor;
    public int GetEvasion() => _evasion;
    public int GetAccuracy() => _accuracy;
    public int GetMovement() => _movement;

    public void ChangeHp(int value)
    {
        _hp += value;
    }
}
