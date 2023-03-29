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
        //public Cell[,] BattleMapCells { get; }
        public Cell[] GetCells();
        public CellRangeBorders CellRangeBorders { get; }
        public Cell this[int x, int y] { get; }
        public Cell this[Vector2Int cellPosition] { get; }
        public Vector2Int GetCellPosition(Cell cell);
        public float GetDistanceBetween(Cell cellA, Cell cellB);
        public Cell GetCell(IAbilitySystemActor actor);
        public Cell GetCell(IBattleObstacle actor);
        public Vector2Int GetCellPosition(IAbilitySystemActor actor);
        public Vector2Int GetCellPosition(IBattleObstacle actor);

        //public IBattleMap GetSideRepresentation(side)
    }
}
