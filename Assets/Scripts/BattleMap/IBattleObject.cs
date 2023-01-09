using CharacterAbility.BuffEffects;
using UnityEngine;

//TODO: Refactor interfaces IBattleObject
public interface IBattleObject : ITickTarget,
    IBuffTarget, IMovable
{
    public BattleObjectSide Side { get; }
    public GameObject View { get; }

    public int GetAccuracyFrom(IBattleObject attacker);
}