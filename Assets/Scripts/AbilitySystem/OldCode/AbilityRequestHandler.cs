using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class AbilityRequestHandler
    {
        
        public Dictionary<IActionTarget, List<IBattleAction>> PreviewAbilityUseActions(
            Ability ability, IActionCasterProcessor caster, CellTargetGroups targetGroups)
        {
            var processedActions = new Dictionary<IActionTarget, List<IBattleAction>>();
            //получение списоков обработанных IBattleActions для каждой цели (считая, что все Actions прошли успешно)
            return processedActions;
        }


        public void UseAbility(
            IBattleContext battleContext, 
            Ability ability, 
            IActionCasterProcessor casterProcessor, 
            IBattleEntity abilityCaster,
            CellTargetGroups targetGroups)
        {
            var useContext = new IAbilityUseContext(ability, casterProcessor, abilityCaster, targetGroups, battleContext);
            foreach (var instruction in ability.ActionInstructions)
                HandleInstruction(instruction, useContext);

            //Цель: обработать рекурсивно все инструкции, получить обработанный список инструкций.
            //var mainCellActions = ability.ActionInstructions.Where(i => i.target = main);
            //foreach (var cell in targetGroups.MainTargets)
            //{
            //    foreach (var entity in cell)
            //    {
            //        if (entity is IAbilityReceiver abilityReceiver)
            //        {

            //        }
            //    }

            //}
            //foreach cell in 
            //Handle Action instructions
            //Apply actions on target
        }

        public void HandleInstruction(ActionInstruction instruction, AbilityUseContext useContext)
        {
            //1. Проверили условия
            //2. Выполнили действие для нужных групп клеток
            //3. Если успешно – рекурсивно по InstructionsOnSuccess
            // А как может рекурсивно вызываться InstructionsOnSuccess, если целевых групп несколько?
            if (instruction.CommonConditions.Any(c => !c.IsConditionMet(useContext.BattleContext, useContext.AbilityCaster)))
                return;
            var affectedCells = instruction.TargetGroupsFilter.GetFilteredCells(useContext.TargetGroups);
            foreach (var cell in affectedCells)
            {
                var actionIsPerformed = false;
                if (instruction.CellConditions.Any(c => !c.IsConditionMet(useContext.BattleContext, useContext.AbilityCaster, cell)))
                    continue;
                foreach (var entity in cell.GetContainingEntities())
                {
                    if (instruction.TargetEntityConditions.Any(c => !c.IsConditionMet(useContext.BattleContext, useContext.AbilityCaster, entity)))
                        continue;
                    if (instruction.Action is IEntityAction entityAction && entityAction.Perform(useContext.BattleContext, useContext.AbilityCaster, entity))
                    {
                        actionIsPerformed = true;
                    }    
                }
                if (actionIsPerformed)
                    foreach (var nextInstruction in instruction.InstructionsOnActionSuccess)
                        HandleInstruction(nextInstruction, useContext);
            }
        }
    }

    public interface IActionCasterProcessor //IBattleEntity
    {
        public IEffect[] Effects { get; }
        //public ProcessedActions, NewActionInstructions ProcessOutcomingAbility(ActionToProcess);
    }

    public interface IActionReceiverProcessor //ITargetable
    {
        //public ProcessedActions, NewActionInstructions ProcessIncomingAction(ActionToProcess, OtherPlannedActionsForThisCharacter);

        //Modify PlannedActions? (Они относятся только к данной сущности) => нет
        //Либо передавать ссылку на дерево выполнения (нет)
        //Нужно вернуть обработанное действие/действия
        //Нужно вернуть инструкции, которые должны быть прикреплены к дереву выполнения
    }
}
