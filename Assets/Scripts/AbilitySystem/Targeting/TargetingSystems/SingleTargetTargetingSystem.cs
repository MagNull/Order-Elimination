using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [Obsolete("\nWill be replaced with \"" + nameof(MultiTargetTargetingSystem) + "\" in the future."
        + "\nRecommended to use interface \"" + nameof(IRequireSelectionTargetingSystem) + "\" instead.")]
    public class SingleTargetTargetingSystem : IAbilityTargetingSystem, IRequireSelectionTargetingSystem
    {
        private List<ICellCondition> _cellConditions;
        private HashSet<Vector2Int> _availableCells;
        private Vector2Int? _selectedCell;
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;
        public bool IsConfirmAvailable => _selectedCell != null && _selectedCell.HasValue;
        public IEnumerable<Vector2Int> CurrentAvailableCells => _availableCells;
        public IEnumerable<Vector2Int> SelectedCells 
            => new Vector2Int?[] { _selectedCell }.Where(c => c.HasValue).Select(c => c.Value);
        public int NecessaryTargetsLeft => _selectedCell == null ? 1 : 0;
        public IReadOnlyList<ICellCondition> CellConditions => _cellConditions;

        public ICellGroupsDistributor CellGroupsDistributor { get; private set; }

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        public SingleTargetTargetingSystem(
            ICellGroupsDistributor groupDistributor, 
            IEnumerable<ICellCondition> cellConditions)
        {
            CellGroupsDistributor = groupDistributor;
            _cellConditions = cellConditions.ToList();//.Clone().ToList() - will prevent real-time changes
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
                Logging.LogException(new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first."));
            _availableCells = PeekAvailableCells(battleContext, caster).ToHashSet();
            _selectedCell = null;
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
            if (!IsTargeting || IsConfirmed)
                return false;
            if (!_availableCells.Contains(cellPosition))
                return false;
            _selectedCell = cellPosition;
            SelectionUpdated?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
        }

        public bool Deselect(Vector2Int cellPosition)
        {
            if (!IsTargeting || IsConfirmed)
                return false;
            if (_selectedCell == cellPosition)
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
            _selectedCell = null;
            _targetingCaster = null;
            _targetingContext = null;
            _availableCells = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public bool TryPeekDistribution(
            out CellGroupsContainer cellGroups,
            IBattleContext battleContext, AbilitySystemActor caster, params Vector2Int[] selectedPositions)
        {
            if (selectedPositions.Length > 1 
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
