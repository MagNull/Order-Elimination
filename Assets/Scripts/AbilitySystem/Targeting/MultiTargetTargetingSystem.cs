using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace OrderElimination.AbilitySystem
{
    public class MultiTargetTargetingSystem : IAbilityTargetingSystem
    {
        public bool IsTargeting { get; private set; }
        public bool IsConfirmed { get; private set; }
        public bool IsConfirmAvailable => IsTargeting && !IsConfirmed && NecessaryTargetsLeft == 0;

        public event Action<IAbilityTargetingSystem> TargetingStarted;
        public event Action<IAbilityTargetingSystem> TargetingConfirmed;
        public event Action<IAbilityTargetingSystem> TargetingCanceled;

        //TODO закрыть set-теры
        //TODO для направленных систем - ограничить паттерны только для целей
        //Если паттерн - CasterRelativePattern, нацеливание не имеет никакого смысла.
        public ICellGroupDistributionPattern TargetPattern
        {
            get => _targetPattern;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value is not TargetRelativePattern or CasterToTargetRelativePattern) throw new ArgumentException();
                _targetPattern = value;
            }
        }
        public int NecessaryTargets
        {
            get => _necessaryTargets;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value <= 0) throw new ArgumentOutOfRangeException();
                _necessaryTargets = value;
            }
        } //1-...
        public int OptionalTargets
        {
            get => _optionalTargets;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value < 0) throw new ArgumentOutOfRangeException();
                _optionalTargets = value;
            }
        }//0-...
        public int NecessaryTargetsLeft => Math.Max(NecessaryTargets - _selectedCells.Count, 0);
        public int TotalTargetsLeft => NecessaryTargets + OptionalTargets - _selectedCells.Count;

        public IEnumerable<Vector2Int> AvailableCells => _availableCells;

        public event Action<MultiTargetTargetingSystem> ConfirmationUnlocked;
        public event Action<MultiTargetTargetingSystem> ConfirmationLocked;
        public event Action<MultiTargetTargetingSystem> SelectionUpdated;

        private ICellGroupDistributionPattern _targetPattern;
        private int _necessaryTargets;
        private int _optionalTargets;
        private HashSet<Vector2Int> _availableCells;
        private readonly List<Vector2Int> _selectedCells = new List<Vector2Int>();
        private Vector2Int? _casterPosition;
        private CellRangeBorders? _mapBorders;

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
            _selectedCells.Clear();
            _casterPosition = null;
            _mapBorders = null;
            _availableCells = null;
            TargetingCanceled?.Invoke(this);
            return true;
        }

        public CellGroupsContainer ExtractCastTargetGroups()
        {
            return TargetPattern.GetAffectedCellGroups(_mapBorders.Value, _casterPosition.Value, _selectedCells.ToArray());
        }
    }
}
