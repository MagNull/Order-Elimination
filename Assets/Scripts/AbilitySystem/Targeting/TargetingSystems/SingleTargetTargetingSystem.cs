using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class SingleTargetTargetingSystem : IAbilityTargetingSystem, IRequireTargetsTargetingSystem
    {
        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;
        public bool IsConfirmAvailable => SelectedCell != null && SelectedCell.HasValue;
        public IEnumerable<Vector2Int> AvailableCells => _availableCells;

        [ShowInInspector, SerializeField]
        public CellGroupDistributionPattern TargetPattern
        {
            get => _targetPattern;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value is not TargetRelativePattern or CasterToTargetRelativePattern) throw new ArgumentException();
                _targetPattern = value;
            }
        }
        public Vector2Int? SelectedCell { get; private set; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;
        public event Action<SingleTargetTargetingSystem> ConfirmationUnlocked;
        public event Action<SingleTargetTargetingSystem> ConfirmationLocked;
        public event Action<SingleTargetTargetingSystem> SelectionUpdated;
        event Action<IRequireTargetsTargetingSystem> IRequireTargetsTargetingSystem.ConfirmationUnlocked
        {
            add => ConfirmationUnlocked += value;

            remove => ConfirmationUnlocked -= value;
        }
        event Action<IRequireTargetsTargetingSystem> IRequireTargetsTargetingSystem.ConfirmationLocked
        {
            add => ConfirmationLocked += value;

            remove => ConfirmationLocked -= value;
        }
        event Action<IRequireTargetsTargetingSystem> IRequireTargetsTargetingSystem.SelectionUpdated
        {
            add => SelectionUpdated += value;

            remove => SelectionUpdated -= value;
        }

        private CellGroupDistributionPattern _targetPattern;
        private HashSet<Vector2Int> _availableCells;
        private Vector2Int? _casterPosition;
        private CellRangeBorders? _mapBorders;

        public SingleTargetTargetingSystem(CellGroupDistributionPattern targetPattern)
        {
            if (targetPattern is not TargetRelativePattern && targetPattern is not CasterToTargetRelativePattern)
                throw new ArgumentException();
            _targetPattern = targetPattern;
        }

        public bool SetAvailableCellsForSelection(Vector2Int[] availableCellsForSelection)
        {
            if (IsTargeting)
                throw new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _availableCells = availableCellsForSelection.ToHashSet();
            return true;
        }

        public bool StartTargeting(CellRangeBorders mapBorders, Vector2Int casterPosition)
        {
            if (IsTargeting || _availableCells == null)
                throw new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            SelectedCell = null;
            _casterPosition = casterPosition;
            _mapBorders = mapBorders;
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
            _casterPosition = null;
            _mapBorders = null;
            _availableCells = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            if (!IsTargeting && !IsConfirmed)
                throw new InvalidOperationException("Targeting is not initiated or being canceled.");
            var selectedCells = new List<Vector2Int>();
            if (SelectedCell.HasValue)
                selectedCells.Add(SelectedCell.Value);
            return TargetPattern.GetAffectedCellGroups(_mapBorders.Value, _casterPosition.Value, selectedCells.ToArray());
        }
    }
}
