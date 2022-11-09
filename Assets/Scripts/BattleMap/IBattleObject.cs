using CharacterAbility.BuffEffects;
using UnityEngine;

public interface IBattleObject : IDamageable, IHealable, ITickTarget, IBuffTarget
{
    public BattleObjectSide Side { get; }
    public GameObject GetView();
}