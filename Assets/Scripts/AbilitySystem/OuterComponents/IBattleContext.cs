using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleContext
{
    public BattleMap BattleMap => throw new System.NotImplementedException();
    //public Player[] Players
    //public Player CurrentPlayer

    //Cell.GetEntities()
    //Cell.GetStructures()
    //Cell.GetContainingObjects()
    public Cell[,] BattleMapCells { get; }
    public float GetDistanceBetween(Cell cellA, Cell cellB);
    public float GetDistanceBetween(IBattleEntity entity, Cell cell); 
    public float GetDistanceBetween(IBattleEntity entityA, IBattleEntity entityB);
}
