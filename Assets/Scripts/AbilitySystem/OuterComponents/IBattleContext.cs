using OrderElimination.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleContext
{
    public IBattleMap BattleMap => throw new NotImplementedException();
    public IHitCalculation HitCalculation => throw new NotImplementedException();
    //public Player[] Players
    //public Player CurrentPlayer
    //Characters
    //Structures

    //Cell.GetEntities()
    //Cell.GetStructures()
    //Cell.GetContainingObjects()

    public int CurrentMove { get; }
    public event Action<int> NewMoveBegun; //MoveInfo(moveNumber, activeSide, ...)
}
