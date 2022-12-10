using OrderElimination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CharacterAbility
{
    public class EffectParameterAttribute : Attribute
    {
        public readonly string DisplayedName;
        public readonly ValueUnits Units;

        public EffectParameterAttribute(string displayedName, ValueUnits units)
        {
            DisplayedName = displayedName;
            Units = units;
        }
    }

    public enum ValueUnits
    {
        None,
        Percents,
        Cells,
        Turns
    }

    public class EffectParameterInfo
    {
        public readonly string PropertyName;
        //public readonly string DisplayedName;//в атрибут
        //public readonly ValueUnits Units;//в атрибут
        public Func<string> ValueGetter;

        public EffectParameterInfo(string name) => PropertyName = name;
    }

    [Serializable]
    public class EffectView
    {
        public string EffectName;
        public Sprite EffectIcon;
        private List<EffectParameterInfo> DisplayedParameters { get; } = new List<EffectParameterInfo>();
        [Tooltip("List of names of Fields and Properties to display.")]
        public List<string> DisplayedProperties = new List<string>();

        private static readonly Dictionary<string, MemberInfo> _properties;

        static EffectView()
        {
            foreach (var f in typeof(AbilityEffect)
                .GetFields()
                .Where(f => Attribute.IsDefined(f, typeof(EffectParameterAttribute))))
            {
                var parameter = f.GetCustomAttribute<EffectParameterAttribute>();
            }
        }

        public EffectView(Sprite icon, string name, List<string> displayedProperties)
        {
            EffectIcon = icon;
            EffectName = name;
            DisplayedProperties = displayedProperties;
        }

        public Dictionary<string, string> GetDisplayingParameters(IReadOnlyBattleStats casterStats)
        {
            var result = new Dictionary<string, string>();
            foreach (var parameterName in DisplayedProperties)
            {
                var units = parameterName;//
            }
            return result;
        }
    }
}
