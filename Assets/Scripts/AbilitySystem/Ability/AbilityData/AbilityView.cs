using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        public IReadOnlyDictionary<int, Color> TargetGroupsHighlightColors { get; }

        public AbilityView(string name, Sprite icon, string description, IReadOnlyDictionary<int, Color> groupColors) 
        {
            Name = name;
            Icon = icon;
            Description = description;
            TargetGroupsHighlightColors = groupColors;
        }
    }
}
