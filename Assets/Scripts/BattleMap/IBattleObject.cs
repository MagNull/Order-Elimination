using UnityEngine;

public interface IBattleObject : IDamageable, IHealable, ITickTarget
{
    public BattleObjectSide Side { get; }
    public GameObject GetView();
}