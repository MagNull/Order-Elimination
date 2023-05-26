using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace OrderElimination.AbilitySystem
{
    public class AbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        //public VideoClip PreviewVideo { get; }
        public IReadOnlyDictionary<int, Color> TargetGroupsHighlightColors { get; }

        public AbilityView(IReadOnlyDictionary<int, Color> groupColors, string name, Sprite icon, string description) 
        {
            Name = name;
            Icon = icon;
            Description = description;
            TargetGroupsHighlightColors = groupColors;
        }
    }
}
