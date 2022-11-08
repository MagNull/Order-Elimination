using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleObject : IDamageable, ITickTarget
{
    public BattleObjectSide Side { get; }
    public GameObject GetView();
}