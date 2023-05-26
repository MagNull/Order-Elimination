using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace OrderElimination.AbilitySystem
{
    [OnInspectorInit("@$value._instructionName = $value._getInstructionName()")]
    [LabelText("@$value." + nameof(_instructionName))]
    public class AbilityInstruction
    {
        #region OdinVisuals
        private string _instructionName;
        private string _getInstructionName()
        {
            var actionName = "...";
            if (Action != null)
                actionName = Action.GetType().Name;
            return $"Instruction <{actionName}>";
        }
        private bool _hasAnyTargetGroups => AffectedCellGroups != null && AffectedCellGroups.Count > 0;
        private bool _actionUtilizesCellGroups => Action is IUtilizeCellGroupsAction;
        private bool _instructionRequireCellGroups
        {
            get
            {
                if (Action == null) return false;
                var actionTarget = Action.ActionRequires;
                var actionRequireCellGroups = actionTarget switch
                {
                    ActionRequires.Entity => true,
                    ActionRequires.Cell => true,
                    //ActionExecutes.OncePerGroup => true,
                    ActionRequires.Nothing => false,
                    _ => throw new NotImplementedException(),
                };
                return actionRequireCellGroups;// && !AffectPreviousTarget;
            }
        }
        #endregion

        private int _repeatNumber = 1;

        [TabGroup("Execution")]
        [GUIColor(1f, 1, 0.2f)]
        [ValidateInput(
            "@!(Action is " + nameof(IUndoableBattleAction) + ")", 
            nameof(AbilityInstruction) + " does not support Undoable actions! Use with caution or utilize effects!")]
        [ShowInInspector, OdinSerialize]
        public IBattleAction Action { get; set; }

        [TabGroup("Targeting")]
        [ShowInInspector, OdinSerialize]
        public bool AffectPreviousTarget { get; set; } = false;

        public List<ICommonCondition> CommonConditions { get; private set; } = new();
        public List<ICellCondition> CellConditions { get; private set; } = new();

        [TabGroup("Targeting")]
        [ShowInInspector, OdinSerialize]
        public List<IEntityCondition> TargetConditions { get; private set; } = new();

        [TabGroup("Targeting")]
        [ShowIf("@" + nameof(_instructionRequireCellGroups))]
        [ValidateInput("@_hasAnyTargetGroups || AffectPreviousTarget", "Instruction has no affected cell groups.")]
        [ShowInInspector, OdinSerialize]
        public HashSet<int> AffectedCellGroups { get; set; } = new();

        [TabGroup("Execution")]
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

        //public bool StopRepeatAfterFirstFail { get; set; }

        //TODO: Single instructions list for following instructions
        //with identifier when to use them (always, on attempt, on success, on fail)

        [GUIColor(0.5f, 1f, 0.5f)]
        [TabGroup("Execution")]
        [ShowInInspector, OdinSerialize]
        public List<AbilityInstruction> InstructionsOnActionSuccess { get; set; } = new();

        [GUIColor(0.7f, 1f, 0.7f)]
        [TabGroup("Execution")]
        [ShowInInspector, OdinSerialize]
        public bool SuccessInstructionsEveryRepeat { get; set; } = true;

        [GUIColor(1f, 0.5f, 0.5f)]
        [TabGroup("Execution")]
        [ShowInInspector, OdinSerialize]
        public List<AbilityInstruction> InstructionsOnActionFail { get; set; } = new();

        [GUIColor(1f, 0.7f, 0.7f)]
        [TabGroup("Execution")]
        [ShowInInspector, OdinSerialize]
        public bool FailInstructionsEveryRepeat { get; set; } = true;

        [GUIColor(0.6f, 0.6f, 1f)]
        [TabGroup("Execution")]
        [ShowInInspector, OdinSerialize]
        public List<AbilityInstruction> FollowingInstructions { get; set; } = new();

        [GUIColor(0.75f, 0.75f, 1f)]
        [TabGroup("Execution")]
        [ShowInInspector, OdinSerialize]
        public bool FollowInstructionsEveryRepeat { get; set; } = true;

        [TabGroup("Animations")]
        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation AnimationBeforeAction { get; set; }
        //AnimationAfterAction
        //AnimationBeforExecution
        //AnimationAfterExecution

        public async UniTask ExecuteRecursive(AbilityExecutionContext executionContext)
        {
            var battleContext = executionContext.BattleContext;
            var battleMap = battleContext.BattleMap;
            var caster = executionContext.AbilityCaster;
            if (CommonConditions != null && !CommonConditions.All(c => c.IsConditionMet(battleContext, caster)))
                return;
            var groups = executionContext.TargetedCellGroups;
            if (Action.ActionRequires == ActionRequires.Nothing)
            {
                if (Action is IUtilizeCellGroupsAction groupAction
                    && !groupAction.UtilizingCellGroups
                    .Where(g => groupAction.GetUtilizedCellsAmount(g) > 0)
                    .All(requiredG => groups.ContainsGroup(requiredG) 
                    && groups.GetGroup(requiredG).Length >= groupAction.GetUtilizedCellsAmount(requiredG)))
                {
                    return;
                }
                await ExecuteConsideringPreviousTarget(executionContext, caster, null);
            }
            else
            {
                if (AffectPreviousTarget)
                    await ExecuteConsideringPreviousTarget(executionContext, caster, null);
                foreach (var pos in executionContext
                .TargetedCellGroups
                .ContainedCellGroups
                .Where(g => AffectedCellGroups.Contains(g))
                .SelectMany(g => executionContext.TargetedCellGroups.GetGroup(g)))
                {
                    var cellConditionsMet = CellConditions == null
                        || CellConditions.All(c => c.IsConditionMet(battleContext, caster, pos));
                    if (Action.ActionRequires == ActionRequires.Cell)
                    {
                        await ExecuteNextInstructions(cellConditionsMet, null, pos);
                    }
                    else if (Action.ActionRequires == ActionRequires.Entity)
                    {
                        var entitiesInCell = battleMap.GetContainedEntities(pos).ToArray();
                        foreach (var entity in entitiesInCell)
                        {
                            var entityConditionsMet = TargetConditions == null
                                || TargetConditions.All(c => c.IsConditionMet(battleContext, caster, entity));
                            await ExecuteNextInstructions(cellConditionsMet && entityConditionsMet, entity, pos);
                        }
                    }
                }
            }

            async UniTask ExecuteNextInstructions(bool conditionsMet, AbilitySystemActor entity, Vector2Int? pos)
            {
                var success = false;
                var newContext = new AbilityExecutionContext(executionContext, entity);
                if (conditionsMet)
                {
                    success = await ExecuteCurrentInstruction(executionContext, caster, entity, pos);
                    if (success && !SuccessInstructionsEveryRepeat)
                        await ExecuteRecursiveInSequence(InstructionsOnActionSuccess, newContext);
                    if (!success && !FailInstructionsEveryRepeat)
                        await ExecuteRecursiveInSequence(InstructionsOnActionFail, newContext);
                }
                if (!FollowInstructionsEveryRepeat)
                    await ExecuteRecursiveInSequence(FollowingInstructions, newContext);
            }

            async UniTask ExecuteConsideringPreviousTarget(
                AbilityExecutionContext executionContext,
                AbilitySystemActor caster,
                AbilitySystemActor entityIfNoPreviousTarget,
                Vector2Int? targetPositionOverride = null)
            {
                var executionEntity = entityIfNoPreviousTarget;
                if (AffectPreviousTarget && executionContext.PreviousInstructionTarget != null)
                    executionEntity = executionContext.PreviousInstructionTarget;
                if (TargetConditions != null
                    && !TargetConditions.All(c => c.IsConditionMet(battleContext, caster, executionEntity)))
                    return;
                var result = await ExecuteCurrentInstruction(executionContext, caster, executionEntity, targetPositionOverride);
            }

            async UniTask<bool> ExecuteCurrentInstruction(
                AbilityExecutionContext executionContext, 
                AbilitySystemActor caster, 
                AbilitySystemActor target,
                Vector2Int? targetPositionOverride = null)
            {
                executionContext = new AbilityExecutionContext(
                    executionContext, 
                    target);
                var entityActionUseContext = new ActionContext(
                    executionContext.BattleContext, 
                    executionContext.TargetedCellGroups, 
                    caster, 
                    target, 
                    targetPositionOverride);
                if (targetPositionOverride == null && target != null)
                {
                    targetPositionOverride = target.Position;
                }
                var animationContext = new AnimationPlayContext(
                    executionContext.AnimationSceneContext,
                    executionContext.TargetedCellGroups,
                    caster,
                    target,
                    targetPositionOverride);
                return await ExecuteActions(executionContext, entityActionUseContext, animationContext);
            }

            async UniTask<bool> ExecuteActions(
                AbilityExecutionContext executionContext, 
                ActionContext actionContext, 
                AnimationPlayContext animationContext)
            {
                var anyActionPerformed = false;
                for (var i = 0; i < RepeatNumber; i++)
                {
                    if (AnimationBeforeAction != null)
                        await AnimationBeforeAction.Play(animationContext);
                    if ((await Action.ModifiedPerform(actionContext)).IsSuccessful) //Action Success
                    {
                        if (SuccessInstructionsEveryRepeat)
                        {
                            await ExecuteRecursiveInSequence(InstructionsOnActionSuccess, executionContext);
                        }
                        anyActionPerformed = true;
                    }
                    else //Action Fail
                    {
                        if (FailInstructionsEveryRepeat)
                        {
                            await ExecuteRecursiveInSequence(InstructionsOnActionFail, executionContext);
                        }
                    }
                    if (FollowInstructionsEveryRepeat)
                    {
                        await ExecuteRecursiveInSequence(FollowingInstructions, executionContext);
                    }
                }
                return anyActionPerformed;
            }

            async UniTask ExecuteRecursiveInSequence(
                IEnumerable<AbilityInstruction> instructions, 
                AbilityExecutionContext executionContext)
            {
                if (instructions == null)
                    return;
                foreach (var instruction in instructions)
                    await instruction.ExecuteRecursive(executionContext);
            }
        }


        //public void AddInstructionsAfterRecursive<TAction>(ActionInstruction instructionToAdd, bool copyParentTargetGroups) where TAction : BattleAction<TAction>
        //{
        //    if (Action is TAction)
        //        InstructionsOnActionSuccess.Add(instructionToAdd);

        //    //TODO фильтры должны меняться у каждой добавленной инструкции при копировании родительской (copyParentTargetGroups = true)
        //    //Должна оставаться возможность выборочно удалить добавленные инструкции
        //    //Клонировать инструкции?
        //    //На крайний случай, можно добавлять все новые инструкции в отдельный список, после чего удалять конкретно их по ссылкам

        //    //if (copyParentTargetGroups) instructionToAdd.TargetGroupsFilter = TargetGroupsFilter; 
        //    foreach (var nextInstruction in InstructionsOnActionSuccess)
        //        nextInstruction.AddInstructionsAfterRecursive<TAction>(instructionToAdd, copyParentTargetGroups);
        //}

        //public void RemoveInstructionsRecursive(ActionInstruction instructionToRemove)
        //{
        //    InstructionsOnActionSuccess.Remove(instructionToRemove);
        //    foreach (var nextInstruction in InstructionsOnActionSuccess)
        //        nextInstruction.RemoveInstructionsRecursive(instructionToRemove);
        //}

        //Возможный вариант получения всех действий для предпросмотра
        //public Dictionary<IAbilitySystemActor, List<IBattleAction>> GetAllPossibleActions(IAbilityUseContext abilityUseContext)
    }
}
