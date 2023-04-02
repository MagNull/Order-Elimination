using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IAbilityCastSystem
    {
        public bool IsTargeting { get; }
        public bool IsConfirmed { get; }
        public bool IsConfirmAvailable { get; }

        public event Action<IAbilityCastSystem> TargetingStarted;
        public event Action<IAbilityCastSystem> TargetingConfirmed;
        public event Action<IAbilityCastSystem> TargetingCanceled;

        public bool StartTargeting(CellRangeBorders mapBorders, Vector2Int casterPosition);
        public bool ConfirmTargeting();
        public bool CancelTargeting();
        public CellGroupsContainer ExtractCastTargetGroups();
    }
}
