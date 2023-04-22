using Assets.AbilitySystem.PrototypeHelpers;
using OrderElimination.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public interface IBattleContext
{
    public IObjectResolver ObjectResolver { get; }
    public IBattleMap BattleMap { get; }
    public IHitCalculation HitCalculation { get; }
    public int CurrentRound { get; }
    public BattleEntitiesBank EntitiesBank { get; }
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
