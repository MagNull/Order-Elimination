using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CasterRelativeCellSelector : ICellSelector
    {
        [ShowInInspector, SerializeField]
        public IPointRelativePattern RelativeToCasterOffsets { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            if (!context.BattleContext.BattleMap.ContainsEntity(context.AskingEntity))
                throw new ArgumentException("Entity doesn't exist on the map");
            return RelativeToCasterOffsets
                .GetAbsolutePositions(context.AskingEntity.Position)
                .Where(p => context.BattleContext.BattleMap.ContainsPosition(p))
                .ToArray();
        }
    }
}
