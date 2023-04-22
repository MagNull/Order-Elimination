using Cysharp.Threading.Tasks;
using Mono.Cecil.Cil;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using static UnityEngine.EventSystems.EventTrigger;

namespace OrderElimination.AbilitySystem
{
    public class ActionInstruction
    {
        private int _repeatNumber = 1;

        public ICommonCondition[] CommonConditions { get; set; }
        public ICellCondition[] CellConditions { get; set; }
        public ITargetCondition[] ActionConditions { get; set; }

        [ShowInInspector, OdinSerialize]
        public HashSet<int> AffectedCellGroups { get; set; } = new();

        //TODO Action нужно дублировать перед обработкой
        [GUIColor(0, 1, 0)]
        [ShowInInspector, OdinSerialize]
        public IBattleAction Action { get; set; }

        //При каждом успешном выполнении Action будут вызываться последующие инструкции (для каждого повторения)
        [ShowInInspector, OdinSerialize]
        public int RepeatNumber
        {
            get => _repeatNumber;
            set
            {
                if (value < 1) value = 1;
                _repeatNumber = value;
            }
        }

        [ShowInInspector, OdinSerialize]
        public List<ActionInstruction> InstructionsOnActionSuccess { get; set; } = new();

        [GUIColor(1f, 0, 0)]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation AnimationBeforeAction { get; set; }

        public async UniTask<bool> ExecuteRecursive(AbilityExecutionContext abilityExecutionContext)
        {
            var battleContext = abilityExecutionContext.BattleContext;
            var battleMap = battleContext.BattleMap;
            //TODO Check Basic Conditions
            var animationSceneContext = battleContext.ObjectResolver.Resolve<AnimationSceneContext>();
            var caster = abilityExecutionContext.AbilityCaster;
            var casterPosition = caster.Position;
            AnimationPlayContext animationContext;
            foreach (var pos in abilityExecutionContext
                .TargetedCellGroups
                .CellGroups
                .Where(e => AffectedCellGroups.Contains(e.Key))
                .SelectMany(e => e.Value))
            {
                //TODO Check Cell Conditions
                //CellActions
                if (Action.ActionTargets == ActionTargets.CellsOnly)
                {
                    var cellActionUseContext = new ActionExecutionContext(
                        battleContext, caster, null, pos);
                    animationContext = new AnimationPlayContext(animationSceneContext, casterPosition, pos, caster, null);
                    for (var i = 0; i < RepeatNumber; i++)
                    {
                        if (AnimationBeforeAction != null)
                            await AnimationBeforeAction.Play(animationContext);
                        if (Action.ModifiedPerform(cellActionUseContext))
                        {
                            foreach (var nextInstruction in InstructionsOnActionSuccess)
                                await nextInstruction.ExecuteRecursive(abilityExecutionContext);
                        }
                    }
                }
                else if (Action.ActionTargets == ActionTargets.EntitiesOnly)
                {
                    var entitiesInCell = battleMap.GetContainingEntities(pos).ToArray();
                    foreach (var entity in entitiesInCell)
                    {
                        //TODO Check Entity Conditions
                        var entityActionUseContext = new ActionExecutionContext(battleContext, caster, entity);
                        animationContext = new AnimationPlayContext(animationSceneContext, casterPosition, pos, caster, entity);
                        for (var i = 0; i < RepeatNumber; i++)
                        {
                            if (AnimationBeforeAction != null)
                                await AnimationBeforeAction.Play(animationContext);
                            if (Action.ModifiedPerform(entityActionUseContext))
                            {
                                foreach (var nextInstruction in InstructionsOnActionSuccess)
                                    await nextInstruction.ExecuteRecursive(abilityExecutionContext);
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void AddInstructionsAfterRecursive<TAction>(ActionInstruction instructionToAdd, bool copyParentTargetGroups) where TAction : BattleAction<TAction>
        {
            if (Action is TAction)
                InstructionsOnActionSuccess.Add(instructionToAdd);

            //TODO фильтры должны меняться у каждой добавленной инструкции при копировании родительской (copyParentTargetGroups = true)
            //Должна оставаться возможность выборочно удалить добавленные инструкции
            //Клонировать инструкции?
            //На крайний случай, можно добавлять все новые инструкции в отдельный список, после чего удалять конкретно их по ссылкам

            //if (copyParentTargetGroups) instructionToAdd.TargetGroupsFilter = TargetGroupsFilter; 
            foreach (var nextInstruction in InstructionsOnActionSuccess)
                nextInstruction.AddInstructionsAfterRecursive<TAction>(instructionToAdd, copyParentTargetGroups);
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
