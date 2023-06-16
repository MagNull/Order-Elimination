using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [Obsolete(
        "Will be replaced with " 
        + nameof(MultiTargetTargetingSystem)
        + " in the future.")]
    public class SingleTargetTargetingSystem : IAbilityTargetingSystem, IRequireSelectionTargetingSystem
    {
        private CellGroupDistributionPattern _targetPattern;
        private HashSet<Vector2Int> _availableCells;
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;
        public bool IsConfirmAvailable => SelectedCell != null && SelectedCell.HasValue;
        public IEnumerable<Vector2Int> CurrentAvailableCells => _availableCells;
        public IEnumerable<Vector2Int> SelectedCells 
            => Enumerable.Repeat(SelectedCell, 1).Where(c => c.HasValue).Select(c => c.Value);

        [ShowInInspector, SerializeField]
        public CellGroupDistributionPattern TargetPattern
        {
            get => _targetPattern;
            set
            {
                if (IsTargeting) Logging.LogException( new InvalidOperationException());
                if (value is not TargetRelativePattern or CasterToTargetRelativePattern) Logging.LogException( new ArgumentException());
                _targetPattern = value;
            }
        }
        public Vector2Int? SelectedCell { get; private set; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        public SingleTargetTargetingSystem(CellGroupDistributionPattern targetPattern)
        {
            if (targetPattern is not TargetRelativePattern && targetPattern is not CasterToTargetRelativePattern)
                Logging.LogException( new ArgumentException());
            _targetPattern = targetPattern;
        }

        public bool SetAvailableCellsForSelection(Vector2Int[] availableCellsForSelection)
        {
            if (IsTargeting)
                Logging.LogException( new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first."));
            _availableCells = availableCellsForSelection.ToHashSet();
            return true;
        }

        public bool StartTargeting(IBattleContext context, AbilitySystemActor caster)
        {
            if (IsTargeting || _availableCells == null)
                Logging.LogException( new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first."));
            SelectedCell = null;
            _targetingCaster = caster;
            _targetingContext = context;
            IsTargeting = true;
            TargetingStarted?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool Select(Vector2Int cell)
        {
            if (!IsTargeting || IsConfirmed)
                return false;
            if (!_availableCells.Contains(cell))
                return false;
            SelectedCell = cell;
            SelectionUpdated?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool Deselect(Vector2Int cell)
        {
            if (!IsTargeting || IsConfirmed)
                return false;
            if (SelectedCell == cell)
            {
                SelectionUpdated?.Invoke(this);
                ConfirmationLocked?.Invoke(this);
                return true;
            }
            return false;
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
            if (!IsTargeting) return false;

            IsTargeting = false;
            IsConfirmed = false;
            SelectedCell = null;
            _targetingCaster = null;
            _targetingContext = null;
            _availableCells = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            if (!IsTargeting && !IsConfirmed)
                Logging.LogException( new InvalidOperationException("Targeting is not initiated or being canceled."));
            var mapBorders = _targetingContext.BattleMap.CellRangeBorders;
            var casterPosition = _targetingCaster.Position;
            return TargetPattern.GetAffectedCellGroups(
                mapBorders, casterPosition, SelectedCells.ToArray());
        }
    }
}
