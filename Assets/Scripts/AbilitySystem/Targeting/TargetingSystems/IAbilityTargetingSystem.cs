﻿using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IAbilityTargetingSystem
    {
        public bool IsTargeting { get; }
        public bool IsConfirmed { get; }
        public bool IsConfirmAvailable { get; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public bool StartTargeting(CellRangeBorders mapBorders, Vector2Int casterPosition);
        public bool ConfirmTargeting();
        public bool CancelTargeting();
        public CellGroupsContainer ExtractCastTargetGroups();
    }

    public interface IRequireTargetsTargetingSystem : IAbilityTargetingSystem
    {
        public IEnumerable<Vector2Int> AvailableCells { get; }

        public event Action<IRequireTargetsTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireTargetsTargetingSystem> ConfirmationLocked;
        public event Action<IRequireTargetsTargetingSystem> SelectionUpdated;

        public bool SetAvailableCellsForSelection(Vector2Int[] availableCellsForSelection);
    }
}
