using OrderElimination.Infrastructure;

namespace OrderElimination.AbilitySystem.UI
{
    public readonly struct HumanValue
    {
        public readonly string Name;
        public readonly string Value;
        public readonly ValueUnits ValueUnits;

        public HumanValue(string parameterName, string value = "", ValueUnits valueUnits = ValueUnits.None)
        {
            Name = parameterName;
            Value = value;
            ValueUnits = valueUnits;
        }

        public HumanValue(string parameterName, float value, ValueUnits valueUnits = ValueUnits.None)
        {
            Name = parameterName;
            Value = value.ToString();
            ValueUnits = valueUnits;
        }

        public HumanValue Rename(string parameterName)
        {
            return new(parameterName, Value, ValueUnits);
        }

        public HumanValue AddPrefix(string prefix)
        {
            return Rename($"{prefix}{Name}");
        }

        public override string ToString()
        {
            var units = Localization.Localization.Current.GetUnits(ValueUnits);
            return $"{Name}: {Value}{units}";
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            hash = (hash ^ Value.GetHashCode()) * 137;
            hash = (hash ^ ValueUnits.GetHashCode()) * 137;
            return hash;
        }
    }
}
