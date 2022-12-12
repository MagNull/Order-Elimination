using CharacterAbility.BuffEffects;
using UnityEngine;

public interface IBattleObject : ITickTarget, IBuffTarget, IMovable
{
    public BattleObjectSide Side { get; }
    public GameObject View { get; }
}