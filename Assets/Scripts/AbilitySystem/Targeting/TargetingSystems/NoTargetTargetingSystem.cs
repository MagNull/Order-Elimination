﻿using OrderElimination.Infrastructure;
using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class NoTargetTargetingSystem : IAbilityTargetingSystem
    {
        public bool IsTargeting { get; private set; }
        public bool IsConfirmed { get; private set; }
        public bool IsConfirmAvailable => IsTargeting && !IsConfirmed;

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public CasterRelativePattern CasterRelativePattern
        {
            get => _casterRelativePattern;
            set
            {
                if (IsTargeting) Logging.LogException( new InvalidOperationException());
                _casterRelativePattern = value;
            }
        }

        private CasterRelativePattern _casterRelativePattern;
        private Vector2Int? _casterPosition;
        private CellRangeBorders? _mapBorders;

        public NoTargetTargetingSystem(CasterRelativePattern casterRelativePattern)
        {
            _casterRelativePattern = casterRelativePattern;
        }

        public bool StartTargeting(CellRangeBorders mapBorders, Vector2Int casterPosition)
        {
            if (IsTargeting)
                return false;
            //Logging.LogException( new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _casterPosition = casterPosition;
            _mapBorders = mapBorders;
            IsTargeting = true;
            TargetingStarted?.Invoke(this);
            return true;
        }

        public bool ConfirmTargeting()
        {
            if (!IsTargeting || IsConfirmed || !IsConfirmAvailable)
                return false;
            IsConfirmed = true;
            TargetingConfirmed?.Invoke(this);
            return true;
        }

        public bool CancelTargeting()
        {
            if (!IsTargeting)
                return false;
            IsTargeting = false;
            IsConfirmed = false;
            _casterPosition = null;
            _mapBorders = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            return CasterRelativePattern.GetAffectedCellGroups(_mapBorders.Value, _casterPosition.Value);
        }
    }
}
