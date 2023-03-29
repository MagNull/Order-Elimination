using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class Ability
    {
        public AbilityView View { get; private set; } //name, icon, effects
        public AbilityGameRepresentation GameRepresentation { get; private set; }
        public AbilityConditions Conditions { get; private set; }
        public AbilityTargeting Targeting { get; private set; }
        AbilityExecution Execution;

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool Cast(IAbilitySystemActor caster, IBattleContext battleContext)
        {
            if (!Conditions.IsAbilityAvailable(caster, GameRepresentation.Cost))
                return false;
            if (!Targeting.StartSelection(battleContext, caster, Conditions.CellConditions))
                return false;
            Targeting.Confirmed += onSelectionConfirmed;
            Targeting.Canceled += onSelectionCanceled;
            return true;
            //Started casting. Now waiting until confirmation.
            //On AddTargetToSelection must give

            void onSelectionConfirmed(AbilityTargeting targetingSystem)
            {
                Targeting.Confirmed -= onSelectionConfirmed;
                Targeting.Canceled -= onSelectionCanceled;
                var executionGroups = Targeting.GetSelectedTargets();
                var abilityUseContext = new AbilityUseContext(battleContext, caster, executionGroups);
                Execution.Use(abilityUseContext);
                Targeting.Cancel();
            }

            void onSelectionCanceled(AbilityTargeting targetingSystem)
            {
                Targeting.Confirmed -= onSelectionConfirmed;
                Targeting.Canceled -= onSelectionCanceled;
            }
        }
    }

    public class AbilityView { }
    public class AbilityGameRepresentation
    {
        public readonly Dictionary<ActionPoint, int> Cost;
        //AbilityTags[] Tags; //Melee, Range, Damage, ...
    }
    public class AbilityConditions
    {
        public ICommonCondition[] AvailabilityConditions;
        public ICellCondition[] CellConditions;
        public ICellPattern Pattern;

        public bool IsAbilityAvailable(IAbilitySystemActor caster, Dictionary<ActionPoint, int> abilityCost)
        {
            if (!IsCostAffordableByCaster(abilityCost, caster))
                return false;
            //Check other conditions
            throw new NotImplementedException();
        }

        private bool IsCostAffordableByCaster(Dictionary<ActionPoint, int> cost, IAbilitySystemActor caster)
        {
            return cost.Keys.All(actionPoint => cost[actionPoint] <= caster.ActionPoints[actionPoint]);
        }
    }
    public class AbilityTargeting
    {
        public enum CellMarkStackingPoicy
        {
            /// <summary>
            /// Допускает назначение клетке нескольких одинаковых или разных меток.
            /// </summary>
            AllowUnlimitedStacking,
            /// <summary>
            /// Допускает назначение клетке нескольких разных меток, не допуская дублирования одинаковых.
            /// </summary>
            ForbidSameMarks,
            /// <summary>
            /// Допускает назначение клетке только одной метки, с наивысшим приоритетом.
            /// </summary>
            OverrideByHigherPriority
        }

        public int NecessaryTargets { get; private set; }//0 (для ненаправленных), 1-...
        public int OptionalTargets { get; private set; }//0-...

        public int NecessaryTargetsLeft => NecessaryTargets - _selectedCells.Count;
        public int TargetsLeftTotal => NecessaryTargets + OptionalTargets - _selectedCells.Count;

        public event Action<AbilityTargeting> SelectionAchieved;
        public event Action<AbilityTargeting> SelectionLost;

        public event Action<AbilityTargeting> Started;
        public event Action<AbilityTargeting> Confirmed;
        public event Action<AbilityTargeting> Canceled;
        public bool IsTargeting { get; private set; }
        public bool IsConfirmed { get; private set; }
        public IEnumerable<Vector2Int> AvailableCells => _availableCells;
        private HashSet<Vector2Int> _availableCells;
        private List<Vector2Int> _selectedCells = new List<Vector2Int>();

        //private selectedTargets

        public AbilityExecutionGroups GetSelectedTargets()
        {
            if (IsTargeting)
                throw new InvalidOperationException("Targeting must be canceled/confirmed before accessing selection");
            throw new NotImplementedException();
        }

        public bool StartSelection(IBattleContext battleContext, IAbilitySystemActor caster, ICellCondition[] cellConditions)
        {
            if (IsTargeting || IsConfirmed)
                throw new InvalidOperationException("Targeting has already started and needs to be confirmed or canceled first.");

            _availableCells = battleContext
                .BattleMap
                //.CellRangeBorders
                //.EnumerateCellPositions()
                .Where(cell => cellConditions.All(c => c.IsConditionMet(battleContext, caster, cell)))
                .Select(cell => battleContext.BattleMap.GetCellPosition(cell))
                .ToHashSet();

            throw new NotImplementedException();
            IsTargeting = true;
        }

        public bool AddToSelection(Vector2Int cell)
        {
            if (TargetsLeftTotal == 0)
                return false;
            if (!_availableCells.Contains(cell))
                return false;
            //Add cell
            //Calculate target groups
            //consider cellmark stacking policy (override by proiority/dont duplicate same marks/allow free stacking)
            //Example: cell - M,M;A;S1,S1;S2,... - marks for Main, Area, Secondary 1, etc.
            if (NecessaryTargetsLeft == 0)
                SelectionAchieved?.Invoke(this);
            //First return true, then invoke event
            return true;
        }

        public void RemoveFromSelection(Vector2Int cell)
        {
            var previousNecessaryTargetsLeft = NecessaryTargets;
            //remove
            var wasRemoved = _selectedCells.Remove(cell);
            //recalculate target groups
            if (previousNecessaryTargetsLeft == 0 && wasRemoved)
                SelectionLost?.Invoke(this);
        }

        public bool Confirm()
        {
            if (NecessaryTargets != 0)
                return false;
            //Lock adding/removing elements to selection
            //Fire Confirmed event
            //DO NOT call Cancel();
            throw new NotImplementedException();
        }

        public bool Cancel()
        {
            throw new NotImplementedException();
            IsTargeting = false;
            //set nulls;
        }
    }

    public class AbilityExecution
    {
        // Действия по зонам(паттерну) способности
        //Изменить на список Группа-Действия ?
        public ActionInstruction[] ActionInstructions;

        public bool StartCast(IBattleContext battleContext, AbilityUseContext useContext)
        {
            //if (!AvailabilityConditions.Any(c => c.IsConditionMet(battleContext, useContext.AbilityCaster)))
            //	return false;
            //         var availableCells = new List<Cell>();
            //foreach (var cell in battleContext.BattleMapCells)
            //	if (CellConditions.All(c => c.IsConditionMet(battleContext, useContext.AbilityCaster)))
            //		availableCells.Add(cell);


            //SelectionSystem.StartSelection(this.abilitySelectionSystem)

            //Battlemap.HighlightCells(availableCells);
            //foreach (var cell in availableCells)
            //	cell.Clicked += 

            //SelectionSystem.SelectionAchieved += On



            //Start
            //1. Show available cells
            //2. Player selects target cell from available
            //3. Ability shows affected cells by CellGroups (Main, Area, Secondary, ...)

            //Processing A
            // foreach Instruction 
            //  take instructionTargets from CellGroups
            //      process

            //Processing B
            // foreach group in CellGroups
            //	foreach cell in group
            //		perform cell actions (spawn object, etc.)
            //		foreach entity in cell

            //Display action/effect information for targets in CellGroups

            //Player Confirms usage

            //Execution
            // Process ApplyingActions on Caster
            // Process ApplyingActions on Target

            //Usage
            //Permorm Actions
            // Run Triggers after usage
            return true;
        }

        public bool StartAbilityCast(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            throw new System.NotImplementedException();
            //if (casting) return false;
            //View.ShowAvailableCells(Conditions)
            //SelectionSystem.SelectionConfirmed += Execution.Use()

            //Игрок взаимодействует с Ability{...} через UI
            //Боты имеют доступ к Ability{...} напрямую

            //Ability.Targeting.StartSelection(Ability.Conditions);
            //после event.Confirmed,
            //(...).GetTargets ->
            //Ability.Execution.Use(targets);
        }

        public void Use(AbilityUseContext abilityUseContext)//TODO return AbilityUseResult
        {
            foreach (var instruction in ActionInstructions)
            {
                instruction.ExecuteRecursive(abilityUseContext);
            }
        }

        public void AddInstructionsAfter<TAction>(ActionInstruction instructionToAdd, bool copyParentTargetGroups) where TAction : IBattleAction
        {
            foreach (var instruction in ActionInstructions)
                instruction.AddInstructionsAfter<TAction>(instructionToAdd, copyParentTargetGroups);
        }

        public void RemoveInstructions(ActionInstruction instructionToRemove)
        {
            foreach (var instruction in ActionInstructions)
                instruction.RemoveInstructionsRecursive(instructionToRemove);
        }
    }

    public class AbilityUseContext
    {
        public readonly IBattleContext BattleContext;
        public readonly IAbilitySystemActor AbilityCaster;
        public readonly AbilityExecutionGroups TargetedCellGroups;

        public AbilityUseContext(IBattleContext battleContext, IAbilitySystemActor abilityCaster, AbilityExecutionGroups targetedCellGroups)
        {
            BattleContext = battleContext;
            AbilityCaster = abilityCaster;
            TargetedCellGroups = targetedCellGroups;
        }
    }
}
