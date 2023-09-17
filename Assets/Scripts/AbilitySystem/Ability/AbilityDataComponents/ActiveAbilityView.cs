using System.Collections.Generic;
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

        public IReadOnlyDictionary<string, IContextValueGetter> CustomParameters { get; } //Range, etc. for manual display

        public ActiveAbilityView(
            IReadOnlyDictionary<int, Color> groupColors,
            IReadOnlyDictionary<string, IContextValueGetter> customParameters,
            string name, Sprite icon, string description, 
            bool hideInCharacterDescription,
            bool showCrosshair,
            bool showTrajectory) 
        {
            Name = name ?? "";
            Icon = icon;
            Description = description ?? "";
            TargetGroupsHighlightColors = groupColors;
            CustomParameters = customParameters;
            HideInCharacterDescription = hideInCharacterDescription;
            ShowCrosshairWhenTargeting = showCrosshair;
            ShowTrajectoryWhenTargeting = showTrajectory;
        }
    }
}
