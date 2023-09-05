using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    /// <summary>
    /// Class which describes an instruction for active ability.
    /// </summary>
    [OnInspectorInit("@$property.State.Expanded = true")]
    [LabelText("@$value." + nameof(InstructionName))]
    public class AbilityInstruction : ICloneable<AbilityInstruction>
    {
        #region OdinVisuals
        private string InstructionName
        {
            get
            {
                var actionName = "...";
                if (Action != null)
                    actionName = Action.GetType().Name;
                return $"Instruction <{actionName}>";
            }
        }
        private bool _hasAnyTargetGroups => _affectedCellGroups != null && _affectedCellGroups.Count > 0;
        private bool IsEntityAction => Action != null ? Action.ActionRequires == ActionRequires.Target : false;
        private bool CellGroupsRequired
        {
            get
            {
                if (Action == null) return false;
                var actionTarget = Action.ActionRequires;
                var actionRequireCellGroups = actionTarget switch
                {
                    ActionRequires.Target => true,
                    ActionRequires.Cell => true,
                    ActionRequires.Maker => false,
                    _ => throw new NotImplementedException(),
                };
                return actionRequireCellGroups;
            }
        }
        #endregion

        private bool _isSafeCopy;

        #region Properties
        private int _repeatNumber = 1;

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Action", CenterLabel = true)]
        [GUIColor(1f, 1, 0.2f)]
        [ValidateInput(
            "@!(Action is " + nameof(IUndoableBattleAction) + ")", 
            "Action is Undoable but can't be undone with " + nameof(AbilityInstruction) + ". Use with caution or utilize effects!")]
        [ShowInInspector, OdinSerialize]
        public IBattleAction Action { get; private set; }

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Action", CenterLabel = true)]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        [ShowInInspector, OdinSerialize]
        public int RepeatNumber //TODO: Rename to Repetitions, place serialization on field
        {
            get => _repeatNumber;
            set
            {
                if (value < 1) value = 1;
                _repeatNumber = value;
            }
        }

        #region Conditions
        [TabGroup("MainSection", "Targeting")]
        [BoxGroup("MainSection/Targeting/Conditions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        private List<ICommonCondition> _commonConditions { get; set; } = new();
        //cell conditions are useless if cell groups are already filtered by conditions
        private List<ICellCondition> _cellConditions { get; set; } = new();

        [BoxGroup("MainSection/Targeting/Conditions", CenterLabel = true)]
        [EnableIf("@" + nameof(IsEntityAction))]
        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _targetConditions { get; set; } = new();
        public IReadOnlyList<IEntityCondition> TargetConditions => _targetConditions;
        #endregion

        [BoxGroup("MainSection/Targeting/Target", CenterLabel = true)]
        [EnableIf(
            "@" + nameof(IsEntityAction) + " && "
            + "$property.ParentValueProperty.ParentValueProperty != null && "//is in list
            + "$property.ParentValueProperty.ParentValueProperty.ParentValueProperty != null && "//list parent
            + "$property.ParentValueProperty.ParentValueProperty.ParentValueProperty.ValueEntry.WeakSmartValue is " + nameof(AbilityInstruction) + " && "
            + "((" + nameof(AbilityInstruction) + ")$property.ParentValueProperty.ParentValueProperty.ParentValueProperty.ValueEntry.WeakSmartValue)." + nameof(IsEntityAction))]
        [ShowInInspector, OdinSerialize]
        public bool AffectPreviousTarget { get; private set; } = false;

        [BoxGroup("MainSection/Targeting/Target", CenterLabel = true)]
        [EnableIf("@" + nameof(CellGroupsRequired))]
        [ValidateInput(
            "@" + nameof(_hasAnyTargetGroups) + " || " + nameof(AffectPreviousTarget), 
            "Instruction has no affected cell groups.")]
        [ShowInInspector, OdinSerialize]
        private HashSet<int> _affectedCellGroups { get; set; } = new();
        public IEnumerable<int> AffectedCellGroups => _affectedCellGroups;

        //public bool StopRepeatAfterFirstFail { get; set; }

        //TODO: Single instructions list for following instructions
        //with identifier when to use them (always, on attempt, on success, on fail)

        #region NextInstructions
        [GUIColor(0.5f, 1f, 0.5f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        private List<AbilityInstruction> _instructionsOnActionSuccess { get; set; } = new();
        public IReadOnlyList<AbilityInstruction> InstructionsOnActionSuccess => _instructionsOnActionSuccess;

        [GUIColor(0.7f, 1f, 0.7f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public bool SuccessInstructionsEveryRepeat { get; private set; } = true;

        [GUIColor(1f, 0.5f, 0.5f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        private List<AbilityInstruction> _instructionsOnActionFail { get; set; } = new();
        public IReadOnlyList<AbilityInstruction> InstructionsOnActionFail => _instructionsOnActionFail;

        [GUIColor(1f, 0.7f, 0.7f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public bool FailInstructionsEveryRepeat { get; private set; } = true;

        [GUIColor(0.6f, 0.6f, 1f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        private List<AbilityInstruction> _followingInstructions { get; set; } = new();
        public IReadOnlyList<AbilityInstruction> FollowingInstructions => _followingInstructions;

        [GUIColor(0.75f, 0.75f, 1f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public bool FollowInstructionsEveryRepeat { get; private set; } = true;
        #endregion

        [TabGroup("MainSection", "Animations")]
        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation AnimationBeforeAction { get; private set; }
        //AnimationAfterAction
        //AnimationBeforExecution
        //AnimationAfterExecution
        #endregion

        public async UniTask ExecuteRecursive(AbilityExecutionContext executionContext)
        {
            var battleContext = executionContext.BattleContext;
            var battleMap = battleContext.BattleMap;
            var caster = executionContext.AbilityCaster;
            if (_commonConditions != null && !_commonConditions.All(c => c.IsConditionMet(battleContext, caster)))
                return;
            var groups = executionContext.TargetedCellGroups;
            if (Action.ActionRequires == ActionRequires.Maker)
            {
                if (Action is IUtilizeCellGroupsAction groupAction
                    && !groupAction.UtilizedCellGroups
                    .All(requiredG => groups.ContainsGroup(requiredG)))
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
                .Where(g => _affectedCellGroups.Contains(g))
                .SelectMany(g => executionContext.TargetedCellGroups.GetGroup(g)))
                {
                    var cellConditionsMet = _cellConditions == null
                        || _cellConditions.All(c => c.IsConditionMet(battleContext, caster, pos));
                    if (Action.ActionRequires == ActionRequires.Cell)
                    {
                        await ExecuteNextInstructions(cellConditionsMet, null, pos);
                    }
                    else if (Action.ActionRequires == ActionRequires.Target)
                    {
                        var entitiesInCell = battleMap.GetContainedEntities(pos).ToArray();
                        foreach (var entity in entitiesInCell)
                        {
                            var entityConditionsMet = _targetConditions == null
                                || _targetConditions.All(c => c.IsConditionMet(battleContext, caster, entity));
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
                        await ExecuteRecursiveInSequence(_instructionsOnActionSuccess, newContext);
                    if (!success && !FailInstructionsEveryRepeat)
                        await ExecuteRecursiveInSequence(_instructionsOnActionFail, newContext);
                }
                if (!FollowInstructionsEveryRepeat)
                    await ExecuteRecursiveInSequence(_followingInstructions, newContext);
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
                if (executionEntity != null && !executionEntity.IsAlive)
                    return;
                if (executionEntity != null
                    && _targetConditions != null
                    && !_targetConditions.All(c => c.IsConditionMet(battleContext, caster, executionEntity)))
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
                if (targetPositionOverride == null && target != null)
                {
                    targetPositionOverride = target.Position;
                }
                var entityActionUseContext = new ActionContext(
                    executionContext.BattleContext, 
                    executionContext.TargetedCellGroups, 
                    caster, 
                    target);//removed argument: targetPositionOverride

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
                    if (Action.ActionRequires == ActionRequires.Target
                        && actionContext.ActionTarget.IsDisposedFromBattle)
                        continue;
                    if ((await Action.ModifiedPerform(actionContext)).IsSuccessful) //Action Success
                    {
                        if (SuccessInstructionsEveryRepeat)
                        {
                            await ExecuteRecursiveInSequence(_instructionsOnActionSuccess, executionContext);
                        }
                        anyActionPerformed = true;
                    }
                    else //Action Fail
                    {
                        if (FailInstructionsEveryRepeat)
                        {
                            await ExecuteRecursiveInSequence(_instructionsOnActionFail, executionContext);
                        }
                    }
                    if (FollowInstructionsEveryRepeat)
                    {
                        await ExecuteRecursiveInSequence(_followingInstructions, executionContext);
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

        public AbilityInstruction Clone()
        {
            var clone = new AbilityInstruction();
            clone.CopySettingsFrom(this);
            clone._isSafeCopy = true;
            return clone;
        }

        //**Doesn't consider settings where next instructions run every repeat or not
        //Inconvenient instruction addition
        //Can return list of appended instructions (considering children)
        public bool AppendInstructionRecursively(
            Predicate<AbilityInstruction> parentSelector, 
            AbilityInstruction newInstruction,
            bool copyParentTargetGroups,
            InstructionFollowType followType)
        {
            if (!_isSafeCopy)
            {
                Logging.Log(new InvalidOperationException("Attempt to modify instruction in template."));
                return false;
            }
            foreach (var child in InstructionsOnActionSuccess
                .Concat(InstructionsOnActionFail)
                .Concat(FollowingInstructions))
            {
                child.AppendInstructionRecursively(
                    parentSelector, newInstruction, copyParentTargetGroups, followType);
            }
            var parent = this;
            if (parentSelector(parent))
            {
                var createdInstruction = newInstruction.Clone();
                if (copyParentTargetGroups)
                    createdInstruction._affectedCellGroups = parent._affectedCellGroups.ToHashSet();
                switch (followType)
                {
                    case InstructionFollowType.OnSuccess:
                        parent._instructionsOnActionSuccess.Add(createdInstruction);
                        break;
                    case InstructionFollowType.OnFailure:
                        parent._instructionsOnActionFail.Add(createdInstruction);
                        break;
                    case InstructionFollowType.AlwaysFollow:
                        parent._followingInstructions.Add(createdInstruction);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return true;
        }

        public bool ModifyInstructionRecursively(
            Predicate<AbilityInstruction> selector,
            Func<AbilityInstruction, AbilityInstruction> modifier)
        {
            if (!_isSafeCopy)
            {
                Logging.Log(new InvalidOperationException("Attempt to modify instruction in template."));
                return false;
            }
            foreach (var child in InstructionsOnActionSuccess
                .Concat(InstructionsOnActionFail)
                .Concat(FollowingInstructions))
            {
                child.ModifyInstructionRecursively(selector, modifier);
            }
            if (selector(this))
            {
                CopySettingsFrom(modifier(this));
            }
            return true;
        }

        private void CopySettingsFrom(AbilityInstruction instruction)
        {
            RepeatNumber = instruction.RepeatNumber;
            Action = instruction.Action.Clone();

            _commonConditions = instruction._commonConditions.DeepClone();
            _cellConditions = instruction._cellConditions.DeepClone();
            _targetConditions = instruction._targetConditions.DeepClone();
            _affectedCellGroups = instruction._affectedCellGroups.ToHashSet();
            AffectPreviousTarget = instruction.AffectPreviousTarget;

            _instructionsOnActionSuccess = instruction._instructionsOnActionSuccess.DeepClone();
            _instructionsOnActionFail = instruction._instructionsOnActionFail.DeepClone();
            _followingInstructions = instruction._followingInstructions.DeepClone();
            SuccessInstructionsEveryRepeat = instruction.SuccessInstructionsEveryRepeat;
            FailInstructionsEveryRepeat = instruction.FailInstructionsEveryRepeat;
            FollowInstructionsEveryRepeat = instruction.FollowInstructionsEveryRepeat;

            AnimationBeforeAction = instruction.AnimationBeforeAction;//Not safe for changes (shared)
        }
    }
}
