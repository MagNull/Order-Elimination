using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public class CellCountInGroupValueGetter : IContextValueGetter
    {
        [ShowInInspector, OdinSerialize]
        public int CellGroupId { get; set; }

        public string DisplayedFormula => $"CellsCount[{CellGroupId}]";

        public IContextValueGetter Clone()
        {
            var clone = new CellCountInGroupValueGetter();
            clone.CellGroupId = CellGroupId;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var cellGroups = context.CellTargetGroups;
            if (!cellGroups.ContainsGroup(CellGroupId))
                return 0;
            return cellGroups.GetGroup(CellGroupId).Length;
        }
    }
}
