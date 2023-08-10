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

        //public IReadOnlyList<string> Parameters { get; } //Range, etc. for manual display

        public ActiveAbilityView(
            IReadOnlyDictionary<int, Color> groupColors, 
            string name, Sprite icon, string description, 
            bool hideInCharacterDescription,
            bool showCrosshair,
            bool showTrajectory) 
        {
            Name = name ?? "";
            Icon = icon;
            Description = description ?? "";
            TargetGroupsHighlightColors = groupColors;
            HideInCharacterDescription = hideInCharacterDescription;
            ShowCrosshairWhenTargeting = showCrosshair;
            ShowTrajectoryWhenTargeting = showTrajectory;
        }
    }
}
