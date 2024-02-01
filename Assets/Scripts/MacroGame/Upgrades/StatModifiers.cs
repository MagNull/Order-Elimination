using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderElimination.MacroGame
{
    [Serializable]
    public class StatModifiers
    {
        [DictionaryDrawerSettings(KeyLabel = "Stat", ValueLabel = "Modifier")]
        [OdinSerialize]
        private Dictionary<BattleStat, ValueModifier> _statModifiers = new();

        public StatModifiers()
        {
            _statModifiers = new();
        }

        public IReadOnlyDictionary<BattleStat, ValueModifier> Modifiers => _statModifiers;

        public StatModifiers(IReadOnlyDictionary<BattleStat, ValueModifier> statModifiers)
        {
            _statModifiers = statModifiers.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public void SetModifier(BattleStat stat, ValueModifier modifier)
            => _statModifiers[stat] = modifier;

        public float ModifyStat(BattleStat stat, float value)
            => _statModifiers.ContainsKey(stat)
            ? _statModifiers[stat].ModifyValue(value)
            : value;

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var stat in _statModifiers.Keys)
            {
                var modifier = _statModifiers[stat];
                builder.AppendLine($"{stat} {modifier.Operation.AsString()} {modifier.Operand}");
            }
            return builder.ToString();
        }
    }
}