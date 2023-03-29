using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //Игрок взаимодействует с Ability.Targeting через UI
    //Боты имеют доступ к Ability.Targeting напрямую
    public class AbilityTargeting
    {
        //TODO закрыть set-теры
        //TODO ненаправленные способности
        //TODO отдельное применение паттерна к кастеру и к целям
        public CasterRelativePattern CasterRelativePattern { get; set; }
        public ICellGroupDistributionPattern TargetPattern { get; set; }
        private int _necessaryTargets; public int NecessaryTargets
        {
            get => _necessaryTargets;
            set
            {
                if (IsTargeting) throw new InvalidOperationException();
                if (value <= 0) throw new ArgumentOutOfRangeException();
                _necessaryTargets = value;
            }
        } //0 (для ненаправленных), 1-...
        private int _optionalTargets; public int OptionalTargets
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
        public int TargetsLeftTotal => NecessaryTargets + OptionalTargets - _selectedCells.Count;

        public bool IsTargeting { get; private set; }
        public bool IsConfirmed { get; private set; }
        public bool IsSelectionAchieved => NecessaryTargetsLeft == 0;

        public IEnumerable<Vector2Int> AvailableCells => _availableCells;

        public event Action<AbilityTargeting> SelectionAchieved;
        public event Action<AbilityTargeting> SelectionLost;
        public event Action<AbilityTargeting> SelectionUpdated;

        public event Action<AbilityTargeting> SelectionStarted;
        public event Action<AbilityTargeting> SelectionConfirmed;
        public event Action<AbilityTargeting> SelectionCanceled;

        private HashSet<Vector2Int> _availableCells;
        private readonly List<Vector2Int> _selectedCells = new List<Vector2Int>();
        private Vector2Int? _casterPosition;
        private CellRangeBorders? _mapBorders;

        public CellGroupsContainer ExtractTargetedGroups()
        {
            //if (CasterRelativePattern != null)
            //var combinedGroups = 
            return TargetPattern.GetAffectedCellGroups(_mapBorders.Value, _casterPosition.Value, _selectedCells.ToArray());
        }
        public bool StartSelection(Vector2Int[] availableCells, CellRangeBorders mapBorders, Vector2Int casterPosition)
        {
            if (IsTargeting || IsConfirmed)
                return false;
                //throw new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");
            _availableCells = availableCells.ToHashSet();
            _casterPosition = casterPosition;
            _mapBorders = mapBorders;
            IsTargeting = true;
            SelectionStarted?.Invoke(this);
            if (IsSelectionAchieved)
                SelectionAchieved?.Invoke(this);
            return true;
        }

        public bool AddToSelection(Vector2Int cell)
        {
            if (!IsTargeting || IsConfirmed || TargetsLeftTotal == 0)
                return false;
            if (!_availableCells.Contains(cell))
                return false;
            _selectedCells.Add(cell);
            SelectionUpdated?.Invoke(this);
            if (IsSelectionAchieved)
                SelectionAchieved?.Invoke(this);
            return true;
        }

        public bool RemoveFromSelection(Vector2Int cell)
        {
            if (!IsTargeting || IsConfirmed)
                return false;
            var previousSelectionAchievedState = IsSelectionAchieved;
            if (_selectedCells.Remove(cell))
            {
                SelectionUpdated?.Invoke(this);
                if (previousSelectionAchievedState && !IsSelectionAchieved)
                    SelectionLost?.Invoke(this);
                return true;
            }
            return false;
        }

        public bool Confirm()
        {
            if (!IsTargeting || IsConfirmed || !IsSelectionAchieved)
                return false;
            IsConfirmed = true;
            SelectionConfirmed?.Invoke(this);
            //DO NOT call Cancel();
            return true;
        }

        public bool Cancel()
        {
            if (!IsTargeting) return false;

            IsTargeting = false;
            IsConfirmed = false;
            _selectedCells.Clear();
            _casterPosition = null;
            _mapBorders = null;
            _availableCells = null;
            SelectionCanceled?.Invoke(this);
            return true;
        }
    }

    public interface ISelectionSystem
    {
        public event Action<ISelectionSystem> SelectionAchieved;

        public CellGroupsContainer ExtractCellGroups(CellRangeBorders mapBorders, );
    }

    public class NoTargetSelectionSystem : ISelectionSystem
    {
        public CasterRelativePattern Pattern { get; set; }
    }

    public class SingleTargetSelectionSystem : ISelectionSystem
    {

    }

    public class MultipleTargetSelectionSystem : ISelectionSystem
    {

    }
}
