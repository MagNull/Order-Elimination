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
        private CellGroupDistributionPattern _targetPattern;
        private List<ICellCondition> _cellConditions;
        private HashSet<Vector2Int> _availableCells;
        private Vector2Int? _selectedCell;
        private IBattleContext _targetingContext;
        private AbilitySystemActor _targetingCaster;

        public CellGroupDistributionPattern TargetPattern
        {
            get => _targetPattern;
            private set
            {
                if (IsTargeting) Logging.LogException(new InvalidOperationException());
                _targetPattern = value;
            }
        }
        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;
        public bool IsConfirmAvailable => _selectedCell != null && _selectedCell.HasValue;
        public IEnumerable<Vector2Int> CurrentAvailableCells => _availableCells;
        public IEnumerable<Vector2Int> SelectedCells 
            => Enumerable.Repeat(_selectedCell, 1).Where(c => c.HasValue).Select(c => c.Value);
        public int NecessaryTargetsLeft => _selectedCell == null ? 1 : 0;

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        public event Action<IRequireSelectionTargetingSystem> ConfirmationUnlocked;
        public event Action<IRequireSelectionTargetingSystem> ConfirmationLocked;
        public event Action<IRequireSelectionTargetingSystem> SelectionUpdated;
        public event Action<IRequireSelectionTargetingSystem> AvailableCellsUpdated;

        public SingleTargetTargetingSystem(
            CellGroupDistributionPattern targetPattern, 
            IEnumerable<ICellCondition> cellConditions)
        {
            _targetPattern = targetPattern;
            _cellConditions = cellConditions.ToList();//.Clone().ToList() - will prevent real-time changes
        }

        public bool StartTargeting(IBattleContext context, AbilitySystemActor caster)
        {
            if (IsTargeting)
                Logging.LogException(new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first."));
            _availableCells = context.BattleMap.CellRangeBorders
                .EnumerateCellPositions()
                .Where(pos => _cellConditions.All(c => c.IsConditionMet(context, caster, pos)))
                .ToHashSet();
            _selectedCell = null;
            _targetingCaster = caster;
            _targetingContext = context;
            IsTargeting = true;
            TargetingStarted?.Invoke(this);
            if (IsConfirmAvailable)
                ConfirmationUnlocked?.Invoke(this);
            return true;
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

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            if (!IsTargeting && !IsConfirmed)
                Logging.LogException(new InvalidOperationException("Targeting is not initiated or being canceled."));
            var mapBorders = _targetingContext.BattleMap.CellRangeBorders;
            var casterPosition = _targetingCaster.Position;
            return TargetPattern.GetAffectedCellGroups(
                mapBorders, casterPosition, SelectedCells.ToArray());
        }
    }
}
