using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellSelectorContext
    {
        public Vector2Int[] PositionsPool { get; }
        public AbilitySystemActor AskingEntity { get; }
        public Vector2Int[] SelectedCellPositions { get; }
        public IBattleContext BattleContext { get; }

        public CellSelectorContext(
            IBattleContext battleContext,
            Vector2Int[] positionsPool,
            AbilitySystemActor askingEntity, 
            Vector2Int[] selectedCellPositions)
        {
            PositionsPool = positionsPool;
            AskingEntity = askingEntity;
            SelectedCellPositions = selectedCellPositions;
            BattleContext = battleContext;
        }
    }
}
