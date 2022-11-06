using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleObject
{
    public BattleObjectSide Side { get; }
    public GameObject GetView();

    void TakeDamage(int damage, int accuracy);
    void TakeHeal(int heal, int accuracy);
    void AddTickEffect(ITickEffect effect);
    void RemoveTickEffect(ITickEffect effect);
    public void OnTurnStart();
}