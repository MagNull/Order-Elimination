using Mono.Cecil.Cil;
using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ActionInstruction
    {
        //TargetCondition(враг/союзник/постройка/...), TargetHasEffectCondition, ...
        public ICommonCondition[] CommonConditions { get; set; } //доступность Action'а
        public ICellCondition[] CellConditions { get; set; } //доступность клетки
        public ITargetCondition[] TargetEntityConditions { get; set; } //доступность сущности (для применения Action)
        
        public ExecutionGroupsFilter TargetGroupsFilter { get; set; } //К каким группам клеткок будет применяться действие(Main,Area,...)
        //TODO Action нужно дублировать перед обработкой
        public IBattleAction Action { get; set; } 
        private int repeatNumber; public int RepeatNumber
        {
            get => repeatNumber;
            set
            {
                if (value < 0) throw new ArgumentException();
                repeatNumber = value;
            }
        } //При каждом успешном выполнении Action будут вызываться последующие инструкции (для каждого повторения)
        public List<ActionInstruction> InstructionsOnActionSuccess { get; } = new List<ActionInstruction>();

        public bool ExecuteRecursive(AbilityUseContext abilityUseContext)
        {
            var battleMap = abilityUseContext.BattleContext.BattleMap;
            //TODO Check Basic Conditions
            foreach (var cell in TargetGroupsFilter
                .GetFilteredCells(abilityUseContext.TargetedCellGroups)
                .Select(pos => battleMap[pos]))
            {
                //TODO Check Cell Conditions
                foreach (var entity in cell.GetContainingEntities())
                {
                    //TODO Check Entity Conditions
                    var actionUseContext = new ActionUseContext(
                        abilityUseContext.BattleContext,
                        abilityUseContext.AbilityCaster,
                        entity);
                    for (var i = 0; i < RepeatNumber; i++)
                    {
                        if (Action.ModifiedPerform(actionUseContext))
                        {
                            foreach (var nextInstruction in InstructionsOnActionSuccess)
                                nextInstruction.ExecuteRecursive(abilityUseContext);
                        }
                    }
                }
            }
            return true;
        }

        public void AddInstructionsAfter<TAction>(ActionInstruction instructionToAdd, bool copyParentTargetGroups) where TAction : IBattleAction
        {
            if (Action is TAction)
                InstructionsOnActionSuccess.Add(instructionToAdd);

            //TODO фильтры должны меняться у каждой добавленной инструкции при копировании родительской (copyParentTargetGroups = true)
            //Должна оставаться возможность выборочно удалить добавленные инструкции
            //Клонировать инструкции?
            //На крайний случай, можно добавлять все новые инструкции в отдельный список, после чего удалять конкретно их по ссылкам

            //if (copyParentTargetGroups) instructionToAdd.TargetGroupsFilter = TargetGroupsFilter; 
            foreach (var nextInstruction in InstructionsOnActionSuccess)
                nextInstruction.AddInstructionsAfter<TAction>(instructionToAdd, copyParentTargetGroups);
        }

        public void RemoveInstructionsRecursive(ActionInstruction instructionToRemove)
        {
            InstructionsOnActionSuccess.Remove(instructionToRemove);
            foreach (var nextInstruction in InstructionsOnActionSuccess)
                nextInstruction.RemoveInstructionsRecursive(instructionToRemove);
        }

        //Возможный вариант получения всех действий для предпросмотра
        //public Dictionary<IAbilitySystemActor, List<IBattleAction>> GetAllPossibleActions(IAbilityUseContext abilityUseContext)
    }
}
