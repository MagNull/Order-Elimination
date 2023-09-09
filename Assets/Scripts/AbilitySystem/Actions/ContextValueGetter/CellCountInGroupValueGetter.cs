using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct CellCountInGroupValueGetter : IContextValueGetter
    {
        [ShowInInspector, OdinSerialize]
        public int CellGroupId { get; set; }

        public string DisplayedFormula => $"CellsCount[{CellGroupId}]";

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return context.CellTargetGroups != null;
        }

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
