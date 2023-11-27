using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem.UI
{
    [HideLabel]
    public class CustomViewParameter
    {
        //[TableColumnWidth(-200)]
        [ShowInInspector, OdinSerialize]
        public string Name { get; set; }

        [TableColumnWidth(330)]
        [ShowInInspector, OdinSerialize]
        public ContextValueGetterFormatString Value { get; set; }
    }
}
