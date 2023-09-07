using OrderElimination.Infrastructure;

namespace OrderElimination.AbilitySystem.UI
{
    public readonly struct HumanValue
    {
        public readonly string ParameterName;
        public readonly string Value;
        public readonly ValueUnits ValueUnits;

        public HumanValue(string parameterName, string value = "", ValueUnits valueUnits = ValueUnits.None)
        {
            ParameterName = parameterName;
            Value = value;
            ValueUnits = valueUnits;
        }

        public HumanValue(string parameterName, float value, ValueUnits valueUnits = ValueUnits.None)
        {
            ParameterName = parameterName;
            Value = value.ToString();
            ValueUnits = valueUnits;
        }

        public HumanValue Rename(string parameterName)
        {
            return new(parameterName, Value, ValueUnits);
        }

        public HumanValue AddPrefix(string prefix)
        {
            return Rename($"{prefix}{ParameterName}");
        }

        public override string ToString()
        {
            var units = Localization.Localization.Current.GetUnits(ValueUnits);
            return $"{ParameterName}: {Value}{units}";
        }
    }
}
