using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
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

        public float GetValue(ActionContext useContext)
        {
            var cellGroups = useContext.TargetCellGroups;
            if (!cellGroups.ContainsGroup(CellGroupId))
                return 0;
            return cellGroups.GetGroup(CellGroupId).Length;
        }
    }
}
