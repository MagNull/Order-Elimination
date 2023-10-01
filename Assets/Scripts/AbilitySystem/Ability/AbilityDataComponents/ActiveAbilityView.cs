using OrderElimination.AbilitySystem.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityView : IAbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        //public VideoClip PreviewVideo { get; }
        public bool HideInCharacterDescription { get; }
        //Active-only
        public IReadOnlyDictionary<int, Color> TargetGroupsHighlightColors { get; }
        public bool ShowCrosshairWhenTargeting { get; }
        //CrosshairType
        //CrosshairTargetGroups
        public bool ShowTrajectoryWhenTargeting { get; }
        public bool ShowPatternRange { get; }

        public bool HideAutoParameters { get; }
        public IReadOnlyList<CustomViewParameter> CustomParameters { get; } //Range, etc. for manual display

        public ActiveAbilityView(
            string name, Sprite icon, string description,
            IReadOnlyDictionary<int, Color> groupColors,
            IEnumerable<CustomViewParameter> customParameters,
            bool hideAutoParameters,
            bool hideInCharacterDescription,
            bool showCrosshair,
            bool showTrajectory,
            bool showRange) 
        {
            Name = name ?? "";
            Icon = icon;
            Description = description ?? "";
            TargetGroupsHighlightColors = groupColors;
            HideAutoParameters = hideAutoParameters;
            CustomParameters = customParameters.ToList();
            HideInCharacterDescription = hideInCharacterDescription;
            ShowCrosshairWhenTargeting = showCrosshair;
            ShowTrajectoryWhenTargeting = showTrajectory;
            ShowPatternRange = showRange;
        }
    }
}
