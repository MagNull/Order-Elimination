using OrderElimination.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleContext
{
    public IBattleMap BattleMap => throw new NotImplementedException();
    public IHitCalculation HitCalculation => throw new NotImplementedException();
    public int CurrentRound { get; }
    public BattleSide GetRelationship(IAbilitySystemActor entityA, IAbilitySystemActor entityB);
    //public Player[] Players
    //public Player CurrentPlayer
    //Characters
    //Structures

    //Cell.GetCharacters()
    //Cell.GetStructures()
    //Cell.GetContainingObjects()

    public event Action<IBattleContext> NewRoundStarted; //MoveInfo(moveNumber, activeSide, ...)
}
