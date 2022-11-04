using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleObject
{
    public BattleObjectSide Side { get; }
    public GameObject GetView();
    public void OnTurnStart();
}
