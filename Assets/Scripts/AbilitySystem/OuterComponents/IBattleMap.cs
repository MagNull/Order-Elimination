using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleMap : IEnumerable<Cell>
    {
        public Cell this[int x, int y] { get; }
        public Cell this[Vector2Int cellPosition] { get; }
        public Cell GetCell(Vector2Int position) => this[position];
        public Cell GetCell(IAbilitySystemActor actor);
        public Cell GetCell(IBattleObstacle actor);

        public Vector2Int GetCellPosition(Cell cell);
        public Vector2Int GetCellPosition(IAbilitySystemActor actor);
        public Vector2Int GetCellPosition(IBattleObstacle obstacle);

        //public Cell[,] BattleMapCells { get; }
        public Cell[] GetCells();
        public CellRangeBorders CellRangeBorders { get; }

        public float GetDistanceBetween(Cell cellA, Cell cellB);
        public bool Move(IAbilitySystemActor movingEntity, Cell destination);
        //cellAvailabilityCondition
        //Нужно определять, может ли путник пройти по клетке (встать в неё)
        public bool HasPathToDestination(IAbilitySystemActor walker, Cell Destination, out Cell[] path); 
        //public IBattleMap GetSideRepresentation(side)
    }
}
