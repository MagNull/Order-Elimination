using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class MultiTargetTargetingSystem : IAbilityTargetingSystem, IRequireSelectionTargetingSystem
    {
        [HideInInspector, SerializeField]
        private CellGroupDistributionPattern _targetPattern;
        [HideInInspector, SerializeField]
        private int _necessaryTargets = 1;
        [HideInInspector, SerializeField]
        private int _optionalTargets;
        private HashSet<Vector2Int> _availableCells;
        private HashSet<Vector2Int> _selectedCells;
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;

        public bool IsConfirmAvailable =>
            IsTargeting && !IsConfirmed && NecessaryTargetsLeft == 0;

        [ShowInInspector]
        public CellGroupDistributionPattern TargetPattern
        {
            get => _targetPattern;
            private set
            {
                if (IsTargeting) Logging.LogException(new InvalidOperationException("Set target while targeting"));
                if (value is not TargetRelativePattern or CasterToTargetRelativePattern) Logging.LogException( new ArgumentException());
                _targetPattern = value;
            }
        }

        [ShowInInspector]
        public int NecessaryTargets
        {
            get => _necessaryTargets;
            private set
            {
                if (IsTargeting) Logging.LogException(new InvalidOperationException("Try set NecessaryTargets while targeting"));
                if (value < 0) value = 0;
                _necessaryTargets = value;
            }
        }

        [ShowInInspector]
        public int OptionalTargets
        {
            get => _optionalTargets;
            set
            {
                if (IsTargeting) Logging.LogException(new InvalidOperationException("Try set OptionalTargets while targeting"));
                if (value < 0) value = 0;
                _optionalTargets = value;
            }
        } //0-...

        public int NecessaryTargetsLeft => Math.Max(NecessaryTargets - _selectedCells.Count, 0);

        public int TotalTargetsLeft => NecessaryTargets + OptionalTargets - _selectedCells.Count;

        public IEnumerable<Vector2Int> CurrentAvailableCells => _availableCells;
        public IEnumerable<Vector2Int> SelectedCells => _selectedCells;

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        public MultiTargetTargetingSystem(CellGroupDistributionPattern targetPattern, int necessaryTargets = 0,
            int optionalTargets = 0)
        {
            if (necessaryTargets < 0
                || optionalTargets < 0
                || targetPattern is not TargetRelativePattern && targetPattern is not CasterToTargetRelativePattern)
                Logging.LogException(new ArgumentException());
            _targetPattern = targetPattern;
            _necessaryTargets = necessaryTargets;
            _optionalTargets = optionalTargets;
        }

        public bool SetAvailableCellsForSelection(Vector2Int[] availableCellsForSelection)
        {
            if (IsTargeting)
                return false;
            _availableCells = availableCellsForSelection.ToHashSet();
            return true;
        }

        public bool StartTargeting(IBattleContext context, AbilitySystemActor caster)
        {
            if (IsTargeting || _availableCells == null)
                return false;
            //Logging.LogException( new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _selectedCells = new();
            _targetingCaster = caster;
            _targetingContext = context;
            IsTargeting = true;
            TargetingStarted?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool AddToSelection(Vector2Int cell)
        {
            if (!IsTargeting || IsConfirmed || TotalTargetsLeft == 0)
                return false;
            if (!_availableCells.Contains(cell))
                return false;
            _selectedCells.Add(cell);
            SelectionUpdated?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool RemoveFromSelection(Vector2Int cell)
        {
            if (!IsTargeting || IsConfirmed)
                return false;
            var previousSelectionAchievedState = IsConfirmAvailable;
            if (_selectedCells.Remove(cell))
            {
                SelectionUpdated?.Invoke(this);
                if (previousSelectionAchievedState && !IsConfirmAvailable)
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
            _selectedCells = null;
            _targetingCaster = null;
            _targetingContext = null;
            _availableCells = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            var mapBorders = _targetingContext.BattleMap.CellRangeBorders;
            var casterPosition = _targetingCaster.Position;
            return TargetPattern.GetAffectedCellGroups(
                mapBorders, casterPosition, SelectedCells.ToArray());
        }
    }
}