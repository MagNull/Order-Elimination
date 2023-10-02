using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace OrderElimination.AbilitySystem.UI
{
    public class ContextValueGetterFormatString
    {
        [ShowInInspector, OdinSerialize]
        public string FormatString { get; set; } = string.Empty;

        [ListDrawerSettings(ShowIndexLabels = true)]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter[] Parameters { get; set; } = new IContextValueGetter[0];

        public string ToFormatString(ValueCalculationContext calculationContext)
        {
            var parameters = Parameters.Select(p => p.GetSimplifiedFormula(calculationContext)).ToArray();
            return string.Format(FormatString, parameters);
        }
    }
}
