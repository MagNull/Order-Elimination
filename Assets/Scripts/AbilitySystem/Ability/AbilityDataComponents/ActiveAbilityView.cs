using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        //public VideoClip PreviewVideo { get; }
        public bool HideInCharacterDiscription { get; }
        //Active-only
        public IReadOnlyDictionary<int, Color> TargetGroupsHighlightColors { get; }
        public bool ShowCrosshairWhenTargeting { get; }
        //CrosshairType
        //CrosshairTargetGroups
        public bool ShowTrajectoryWhenTargeting { get; }

        public ActiveAbilityView(
            IReadOnlyDictionary<int, Color> groupColors, 
            string name, Sprite icon, string description, 
            bool hideInCharacterDiscription,
            bool showCrosshair,
            bool showTrajectory) 
        {
            Name = name;
            Icon = icon;
            Description = description ?? "";
            TargetGroupsHighlightColors = groupColors;
            HideInCharacterDiscription = hideInCharacterDiscription;
            ShowCrosshairWhenTargeting = showCrosshair;
            ShowTrajectoryWhenTargeting = showTrajectory;
        }
    }
}
