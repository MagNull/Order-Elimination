using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //public class Ability
    //{
    //    AbilityView View; //name, icon, effects
    //    AbilityTags[] Tags; //Melee, Range, Damage, ...
    //    AbilityModel Model;
    //}

    public class Ability
    {
        public ICommonCondition[] AvailabilityConditions;
        public ICellCondition[] CellConditions;
        public ICellPattern Pattern;
        // Действия по зонам(паттерну) способности
        //Изменить на список Группа-Действия ?
        public ActionInstruction<Cell>[] CellActionInstructions;
        public ActionInstruction<IBattleEntity>[] EntityActionInstructions;

        public bool StartCast(IBattleContext battleContext, IAbilityUseContext useContext)
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

        public void Use(IAbilityUseContext abilityUseContext)
        {
            var actionUseContext = new ActionUseContext(abilityUseContext.BattleContext);
            var entityActions = new Dictionary<IBattleEntity, List<IBattleAction<IBattleEntity>>>();

            foreach (var instruction in CellActionInstructions)
            {
                foreach (var cell in instruction.TargetGroupsFilter.GetFilteredCells(abilityUseContext.TargetedCellGroups))
                {
                    //TODO Check Cell Conditions
                    //TODO Подписать последующие Actions
                    instruction.Action.Perform(actionUseContext, abilityUseContext.AbilityCaster, cell);
                }
            }

            foreach (var instruction in EntityActionInstructions)
            {
                //Cells that current instruction will run on
                foreach (var cell in instruction.TargetGroupsFilter.GetFilteredCells(abilityUseContext.TargetedCellGroups))
                {
                    foreach (var entity in cell.GetContainingEntities())
                    {
                        //TODO Check Entity Conditions
                        //TODO Подписать последующие Actions
                        if (!entityActions.ContainsKey(entity))
                            entityActions.Add(entity, new List<IBattleAction<IBattleEntity>>());
                        entityActions[entity]?.Add(instruction.Action);
                        instruction.Action.Perform(actionUseContext, abilityUseContext.AbilityCaster, entity);
                    }
                }
            }
        }

        public void HandleEntityInstruction(ActionInstruction<IBattleEntity> instruction)
        {

        }

        public void HandleCellInstruction(ActionInstruction<IBattleEntity> instruction)
        {
            foreach (var cell in instruction.TargetGroupsFilter.GetFilteredCells(abilityUseContext.TargetedCellGroups))
            {
                //TODO Check Cell Conditions
                //TODO Подписать последующие Actions
                instruction.Action.Perform(actionUseContext, abilityUseContext.AbilityCaster, cell);
            }
        }
    }
    public interface IAbilityUseContext
    {
        IBattleContext BattleContext { get; }
        IBattleEntity AbilityCaster { get; }
        CellTargetGroups TargetedCellGroups { get; }
    }
}
