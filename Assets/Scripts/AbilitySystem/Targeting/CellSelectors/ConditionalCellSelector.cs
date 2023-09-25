using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ConditionalCellSelector : ICellSelector
    {
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] CommonConditions { get; private set; } = new ICommonCondition[0];

        [ShowInInspector, OdinSerialize]
        public ICellCondition[] CellConditions { get; private set; } = new ICellCondition[0];

        [ShowInInspector, OdinSerialize]
        public ICellSelector Source { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var battleContext = context.BattleContext;
            var askingEntity = context.AskingEntity;
            if (CommonConditions != null)
            {
                var conditionsMet = CommonConditions.AllMet(battleContext, askingEntity);
                if (!conditionsMet)
                    return new Vector2Int[0];
            }
            return Source
                .GetCellPositions(context)
                .Where(p => context.BattleContext.BattleMap.ContainsPosition(p))
                .Where(p => CellConditions.AllMet(battleContext, askingEntity, p))
                .ToArray();
        }
    }
}
