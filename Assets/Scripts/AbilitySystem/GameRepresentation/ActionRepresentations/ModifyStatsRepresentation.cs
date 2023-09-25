using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class ModifyStatsRepresentation
    {
        public ModifyStatsRepresentation(ModifyStatsAction modifyStatsAction)
        {
            UnprocessedAction = modifyStatsAction;
        }

        public BattleStat TargetBattleStat => UnprocessedAction.TargetBattleStat;
        public IContextValueGetter UnprocessedValueModifier => UnprocessedAction?.ValueModifier;
        public ModifyStatsAction UnprocessedAction { get; }


    }
}
