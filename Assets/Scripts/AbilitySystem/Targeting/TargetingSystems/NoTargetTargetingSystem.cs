using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class NoTargetTargetingSystem : IAbilityTargetingSystem
    {
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

        public bool IsTargeting { get; private set; }
        public bool IsConfirmed { get; private set; }
        public bool IsConfirmAvailable => IsTargeting && !IsConfirmed;
        public ICellGroupsDistributor CellGroupsDistributor { get; private set; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public NoTargetTargetingSystem(ICellGroupsDistributor cellDistributor)
        {
            CellGroupsDistributor = cellDistributor;
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

        public CellGroupsContainer PeekDistribution(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return CellGroupsDistributor.DistributeSelection(
                battleContext, caster, new Vector2Int[0]);
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            return CellGroupsDistributor.DistributeSelection(
                _targetingContext, _targetingCaster, new Vector2Int[0]);
        }
    }
}
