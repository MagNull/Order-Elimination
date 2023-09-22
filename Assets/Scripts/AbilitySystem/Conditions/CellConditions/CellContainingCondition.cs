using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellContainingCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public bool MustBeEmpty { get; private set; }

        [DisableIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool AllowEmptyCells { get; private set; }

        [DisableIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; } = new IEntityCondition[0];

        [DisableIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool VisibleEntitiesOnly { get; private set; } = true;

        [DisableIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool AllEntitiesMustMeetRequirements { get; private set; }

        public ICellCondition Clone()
        {
            var clone = new CellContainingCondition();
            clone.MustBeEmpty = MustBeEmpty;
            clone.EntityConditions = EntityConditions?.DeepClone();
            clone.VisibleEntitiesOnly = VisibleEntitiesOnly;
            clone.AllowEmptyCells = AllowEmptyCells;
            clone.AllEntitiesMustMeetRequirements = AllEntitiesMustMeetRequirements;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
        {
            var cellEntities = VisibleEntitiesOnly
                ? battleContext.GetVisibleEntitiesAt(positionToCheck, askingEntity.BattleSide).ToArray()
                : battleContext.BattleMap.GetContainedEntities(positionToCheck).ToArray();
            var isCellEmpty = cellEntities.Length == 0;
            if (MustBeEmpty)
                return isCellEmpty;
            if (AllowEmptyCells && isCellEmpty) 
                return true;
            if (AllEntitiesMustMeetRequirements)
            {
                return cellEntities.All(e => EntityConditions.AllMet(battleContext, askingEntity, e));
            }
            else
            {
                var view = battleContext.EntitiesBank.GetViewByEntity(askingEntity);
                if (EntityConditions == null)
                    Debug.LogError($"{nameof(EntityConditions)} null on {view.Name}");
                return cellEntities.Any(e => EntityConditions.AllMet(battleContext, askingEntity, e));
            }
        }
    }
}
