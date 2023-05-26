using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class MultiTargetTargetingSystem : IAbilityTargetingSystem, IRequireTargetsTargetingSystem
    {
        public bool IsTargeting { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;
        public bool IsConfirmAvailable => IsTargeting && !IsConfirmed && NecessaryTargetsLeft == 0;//TODO закрыть set-теры

        //TODO для направленных систем - ограничить паттерны только для целей
        //Если паттерн - CasterRelativePattern, нацеливание не имеет никакого смысла.
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

        [ShowInInspector, SerializeField]
        public int NecessaryTargets
        {
            get => _necessaryTargets;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value < 0) value = 0;
                _necessaryTargets = value;
            }
        } //1-...

        [ShowInInspector, SerializeField]
        public int OptionalTargets
        {
            get => _optionalTargets;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value < 0) value = 0;
                _optionalTargets = value;
            }
        }//0-...

        public int NecessaryTargetsLeft => Math.Max(NecessaryTargets - _selectedCells.Count, 0);

        public int TotalTargetsLeft => NecessaryTargets + OptionalTargets - _selectedCells.Count;

        public IEnumerable<Vector2Int> AvailableCells => _availableCells;

        public IEnumerable<Vector2Int> SelectedCells => _selectedCells;

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;
        public event Action<MultiTargetTargetingSystem> ConfirmationUnlocked;
        public event Action<MultiTargetTargetingSystem> ConfirmationLocked;
        public event Action<MultiTargetTargetingSystem> SelectionUpdated;
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
        private int _necessaryTargets;
        private int _optionalTargets;
        private HashSet<Vector2Int> _availableCells;
        private HashSet<Vector2Int> _selectedCells;
        private Vector2Int? _casterPosition;
        private CellRangeBorders? _mapBorders;

        public MultiTargetTargetingSystem(CellGroupDistributionPattern targetPattern, int necessaryTargets = 0, int optionalTargets = 0)
        {
            if (necessaryTargets < 0 
                || optionalTargets < 0 
                || targetPattern is not TargetRelativePattern && targetPattern is not CasterToTargetRelativePattern)
                throw new ArgumentException();
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

        public bool StartTargeting(CellRangeBorders mapBorders, Vector2Int casterPosition)
        {
            if (IsTargeting || _availableCells == null)
                return false;
            //throw new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _selectedCells = new();
            _casterPosition = casterPosition;
            _mapBorders = mapBorders;
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
            _casterPosition = null;
            _mapBorders = null;
            _availableCells = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            return TargetPattern.GetAffectedCellGroups(
                _mapBorders.Value, _casterPosition.Value, _selectedCells.ToArray());
        }
    }
}
