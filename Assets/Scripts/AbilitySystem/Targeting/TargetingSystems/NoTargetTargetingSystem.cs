using OrderElimination.Infrastructure;
using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class NoTargetTargetingSystem : IAbilityTargetingSystem
    {
        private CasterRelativePattern _casterRelativePattern;
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

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

        public NoTargetTargetingSystem(CasterRelativePattern casterRelativePattern)
        {
            _casterRelativePattern = casterRelativePattern;
        }

        public bool StartTargeting(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (IsTargeting)
                return false;
            //Logging.LogException( new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _targetingCaster = caster;
            _targetingContext = battleContext;
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
            _targetingCaster = null;
            _targetingContext = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            var mapBorders = _targetingContext.BattleMap.CellRangeBorders;
            var casterPosition = _targetingCaster.Position;
            return CasterRelativePattern.GetAffectedCellGroups(mapBorders, casterPosition);
        }
    }
}
