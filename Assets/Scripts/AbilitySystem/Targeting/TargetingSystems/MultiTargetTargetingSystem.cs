using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class MultiTargetTargetingSystem : IAbilityTargetingSystem, IRequireSelectionTargetingSystem
    {
        private List<ICellCondition> _cellConditions;
        private int _necessaryTargets;
        private int _optionalTargets;
        private HashSet<Vector2Int> _availableCells;
        private HashSet<Vector2Int> _selectedCells;
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;
        public bool IsConfirmAvailable =>
            IsTargeting && !IsConfirmed && NecessaryTargetsLeft == 0;

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
        public int OptionalTargets
        {
            get => _optionalTargets;
            private set
            {
                if (IsTargeting) Logging.LogException(new InvalidOperationException("Attempted set OptionalTargets while targeting"));
                if (value < 0) value = 0;
                _optionalTargets = value;
            }
        }
        public int NecessaryTargetsLeft => Math.Max(NecessaryTargets - _selectedCells.Count, 0);
        public int TotalTargetsLeft => NecessaryTargets + OptionalTargets - _selectedCells.Count;
        public IEnumerable<Vector2Int> CurrentAvailableCells => _availableCells;
        public IEnumerable<Vector2Int> SelectedCells => _selectedCells;
        public IReadOnlyList<ICellCondition> CellConditions => _cellConditions;

        public ICellGroupsDistributor CellGroupsDistributor { get; private set; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        public MultiTargetTargetingSystem(
            ICellGroupsDistributor groupDistributor, 
            IEnumerable<ICellCondition> cellConditions,
            int necessaryTargets = 0,
            int optionalTargets = 0)
        {
            if (necessaryTargets < 0
                || optionalTargets < 0)
                Logging.LogException(new ArgumentOutOfRangeException());
            CellGroupsDistributor = groupDistributor;
            _cellConditions = cellConditions.ToList();
            _necessaryTargets = necessaryTargets;
            _optionalTargets = optionalTargets;
        }

        public Vector2Int[] PeekAvailableCells(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (CurrentAvailableCells != null)
                return CurrentAvailableCells.ToArray();
            return battleContext.BattleMap.CellRangeBorders
                .EnumerateCellPositions()
                .Where(pos => _cellConditions.AllMet(battleContext, caster, pos))
                .ToArray();
        }

        public bool StartTargeting(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (IsTargeting)
                return false;
            //Logging.LogException(new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _availableCells = PeekAvailableCells(battleContext, caster).ToHashSet();
            _selectedCells = new();
            _targetingCaster = caster;
            _targetingContext = battleContext;
            IsTargeting = true;
            TargetingStarted?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool CanSelectPeek(
            IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition)
        {
            return battleContext.BattleMap.ContainsPosition(cellPosition)
                && _cellConditions.AllMet(battleContext, caster, cellPosition);
        }

        public bool Select(Vector2Int cellPosition)
        {
            if (!IsTargeting || IsConfirmed || TotalTargetsLeft == 0)
                return false;
            if (!_availableCells.Contains(cellPosition))
                return false;
            _selectedCells.Add(cellPosition);
            SelectionUpdated?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool Deselect(Vector2Int cellPosition)
        {
            if (!IsTargeting || IsConfirmed)
                return false;
            var previousSelectionAchievedState = IsConfirmAvailable;
            if (_selectedCells.Remove(cellPosition))
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

        public bool TryPeekDistribution(out CellGroupsContainer cellGroups, IBattleContext battleContext, AbilitySystemActor caster, params Vector2Int[] selectedPositions)
        {
            if (selectedPositions.Length < NecessaryTargets
                || selectedPositions.Length > NecessaryTargets + OptionalTargets
                || selectedPositions.Any(p => !CanSelectPeek(battleContext, caster, p)))
            {
                cellGroups = CellGroupsContainer.Empty;
                return false;
            }
            cellGroups = CellGroupsDistributor.DistributeSelection(battleContext, caster, selectedPositions);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            return CellGroupsDistributor.DistributeSelection(
                _targetingContext, _targetingCaster, SelectedCells);
        }
    }
}