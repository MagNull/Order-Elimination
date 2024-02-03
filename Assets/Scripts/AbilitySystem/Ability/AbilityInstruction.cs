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
    public class AbilityInstruction : IAbilityInstruction, ICloneable<AbilityInstruction>
    {
        #region OdinVisuals

        private const string HasEntityParentInstruction =
            "$property.ParentValueProperty.ParentValueProperty != null" //instruction parent (list*) not null
            + " && $property.ParentValueProperty.ParentValueProperty.ParentValueProperty != null" //list* parent not null
            + " && $property.ParentValueProperty.ParentValueProperty.ParentValueProperty.ValueEntry.WeakSmartValue is " +
            nameof(AbilityInstruction) //list parent is Instruction
            + " && ((" + nameof(AbilityInstruction) +
            ")$property.ParentValueProperty.ParentValueProperty.ParentValueProperty.ValueEntry.WeakSmartValue)." +
            nameof(IsEntityAction);

        private const string HasCellParentInstruction =
            "$property.ParentValueProperty.ParentValueProperty != null" //instruction parent (list*) not null
            + " && $property.ParentValueProperty.ParentValueProperty.ParentValueProperty != null" //list* parent not null
            + " && $property.ParentValueProperty.ParentValueProperty.ParentValueProperty.ValueEntry.WeakSmartValue is " +
            nameof(AbilityInstruction) //list parent is Instruction
            + " && ((" + nameof(AbilityInstruction) +
            ")$property.ParentValueProperty.ParentValueProperty.ParentValueProperty.ValueEntry.WeakSmartValue)." +
            nameof(IsCellAction);

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
        private bool IsCellAction => Action != null && Action.BattleActionType == BattleActionType.CellAction;
        private bool IsEntityAction => Action != null && Action.BattleActionType == BattleActionType.EntityAction;

        private bool CellGroupsRequired
        {
            get
            {
                if (Action == null) return false;
                var actionTarget = Action.BattleActionType;
                var actionRequireCellGroups = actionTarget switch
                {
                    BattleActionType.EntityAction => true,
                    BattleActionType.CellAction => true,
                    BattleActionType.CommonAction => false,
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
            "Action is Undoable but can't be undone with " + nameof(AbilityInstruction) +
            ". Use with caution or utilize effects!")]
        [ShowInInspector, OdinSerialize]
        public IBattleAction Action { get; private set; }

        //public bool IgnoreCasterProcessing { get; private set; }
        //public bool IgnoreTargetProcessing { get; private set; }

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Action", CenterLabel = true)]
        [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
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
        [ShowInInspector]
        public BattleActionType ActionType => Action?.BattleActionType ?? BattleActionType.CommonAction;

        [BoxGroup("MainSection/Targeting/Target")]
        [EnableIf(
            "@(" + nameof(IsCellAction) + " || " + nameof(IsEntityAction) + ") && (" + HasEntityParentInstruction + ")"
            + " || " + nameof(IsCellAction) + " && (" + HasCellParentInstruction + ")")]
        [ShowInInspector, OdinSerialize]
        public bool AffectPreviousTarget { get; private set; } = false;

        [BoxGroup("MainSection/Targeting/Target")]
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

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [GUIColor(0.5f, 1f, 0.5f)]
        [ShowInInspector, OdinSerialize]
        private List<AbilityInstruction> _instructionsOnActionSuccess { get; set; } = new();

        public IReadOnlyList<AbilityInstruction> InstructionsOnActionSuccess => _instructionsOnActionSuccess;


        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [GUIColor(0.7f, 1f, 0.7f)]
        [PropertyTooltip("If true, executes after each successful action perform.\n"
                         + "If false, executes if AT LEAST ONE perform was successful. ")]
        [EnableIf("@" + nameof(RepeatNumber) + " > 1")]
        //[DisableIf("@" + nameof(_instructionsOnActionSuccess) + ".Count == 0")]
        [ShowInInspector, OdinSerialize]
        public bool SuccessInstructionsEveryRepeat { get; private set; } = true;

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [GUIColor(1f, 0.5f, 0.5f)]
        [ShowInInspector, OdinSerialize]
        private List<AbilityInstruction> _instructionsOnActionFail { get; set; } = new();

        public IReadOnlyList<AbilityInstruction> InstructionsOnActionFail => _instructionsOnActionFail;

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [GUIColor(1f, 0.7f, 0.7f)]
        [PropertyTooltip("If true, executes after each failed action perform.\n"
                         + "If false, executes if ALL performs failed. ")]
        [EnableIf("@" + nameof(RepeatNumber) + " > 1")]
        //[DisableIf("@" + nameof(_instructionsOnActionFail) + ".Count == 0")]
        [ShowInInspector, OdinSerialize]
        public bool FailInstructionsEveryRepeat { get; private set; } = true;

        [GUIColor(0.6f, 0.6f, 1f)]
        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        private List<AbilityInstruction> _followingInstructions { get; set; } = new();

        public IReadOnlyList<AbilityInstruction> FollowingInstructions => _followingInstructions;

        [TabGroup("MainSection", "Execution")]
        [BoxGroup("MainSection/Execution/Next Instructions", CenterLabel = true)]
        [GUIColor(0.75f, 0.75f, 1f)]
        [PropertyTooltip("If true, executes after each action perform.\n"
                         + "If false, executes once.\n" +
                         "Perform result doesn't matter.")]
        [EnableIf("@" + nameof(RepeatNumber) + " > 1")]
        //[DisableIf("@" + nameof(_followingInstructions) + ".Count == 0")]
        [ShowInInspector, OdinSerialize]
        public bool FollowInstructionsEveryRepeat { get; private set; } = true;

        #endregion

        [TabGroup("MainSection", "Animations")]
        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation AnimationBeforeAction { get; private set; }

        //AnimationAfterAction
        //AnimationBeforeExecution
        //AnimationAfterExecution

        #endregion

        public async UniTask ExecuteRecursive(AbilityExecutionContext executionContext)
        {
            var battleContext = executionContext.BattleContext;
            var battleMap = battleContext.BattleMap;
            var caster = executionContext.AbilityCaster;
            if (_commonConditions != null &&
                !_commonConditions.AllMet(battleContext, caster, executionContext.CellTargetGroups))
                return;
            var groups = executionContext.CellTargetGroups;
            if (Action.BattleActionType == BattleActionType.CommonAction)
            {
                if (Action is IUtilizeCellGroupsAction groupAction
                    && !groupAction.UtilizedCellGroups
                        .All(requiredG => groups.ContainsGroup(requiredG))) //why?
                {
                    return;
                }

                await ExecuteCurrentInstruction(executionContext, caster, null, null);
            }
            else if (Action.BattleActionType == BattleActionType.CellAction ||
                     Action.BattleActionType == BattleActionType.EntityAction)
            {
                if (AffectPreviousTarget && executionContext.HasSpecifiedTarget)
                    await ExecuteCurrentInstruction(
                        executionContext, caster, executionContext.SpecifiedCell, executionContext.SpecifiedEntity);
                //Targets in Cell Groups
                foreach (var pos in executionContext
                             .CellTargetGroups
                             .ContainedCellGroups
                             .Where(g => _affectedCellGroups.Contains(g))
                             .SelectMany(g => executionContext.CellTargetGroups.GetGroup(g))
                             .Where(p => _cellConditions == null ||
                                         _cellConditions.AllMet(battleContext, caster, p, groups))
                             .ToArray())
                {
                    if (Action.BattleActionType == BattleActionType.CellAction)
                    {
                        await ExecuteCurrentInstruction(executionContext, caster, pos, null);
                    }
                    else if (Action.BattleActionType == BattleActionType.EntityAction)
                    {
                        var entitiesInCell = battleMap.GetContainedEntities(pos)
                            .Where(e => _targetConditions == null ||
                                        _targetConditions.AllMet(battleContext, caster, e, groups))
                            .ToArray();
                        foreach (var entity in entitiesInCell)
                        {
                            await ExecuteCurrentInstruction(executionContext, caster, pos, entity);
                        }
                    }
                }
            }
            else
                throw new NotImplementedException();

            #region SupportFunctions

            async UniTask ExecuteCurrentInstruction(
                AbilityExecutionContext executionContext,
                AbilitySystemActor caster,
                Vector2Int? targetCell,
                AbilitySystemActor targetEntity)
            {
                if (targetEntity != null)
                    executionContext = executionContext.SpecifyEntity(targetEntity);
                else if (targetCell != null)
                    executionContext = executionContext.SpecifyCell(targetCell.Value);

                var actionContext = ActionContext.CreateBest(
                    executionContext.CallOrigin,
                    executionContext.BattleContext,
                    executionContext.CellTargetGroups,
                    caster, targetCell, targetEntity);
                var animationContext = new AnimationPlayContext(
                    executionContext.AnimationSceneContext,
                    executionContext.CellTargetGroups,
                    caster,
                    targetEntity,
                    actionContext.TargetCell);

                await ExecuteCurrentAndNextInstructions(executionContext, actionContext, animationContext);
            }

            async UniTask ExecuteCurrentAndNextInstructions(
                AbilityExecutionContext executionContext,
                ActionContext actionContext,
                AnimationPlayContext animationContext)
            {
                var anyRepetitionSuccessed = false;

                for (var i = 0; i < RepeatNumber; i++)
                {
                    if (_commonConditions != null && !_commonConditions.AllMet(
                            actionContext.BattleContext,
                            actionContext.ActionMaker))
                        continue;
                    if (Action.BattleActionType == BattleActionType.CellAction
                        || Action.BattleActionType == BattleActionType.EntityAction)
                    {
                        if (actionContext.TargetCell == null)
                        {
                            Logging.LogException(
                                new ArgumentNullException(
                                    "Action requires cell position but it's null. Skipping perform."));
                            continue; //break;
                        }

                        if (_cellConditions != null && !_cellConditions.AllMet(
                                actionContext.BattleContext,
                                actionContext.ActionMaker,
                                actionContext.TargetCell.Value))
                            continue; //return false;
                    }

                    if (Action.BattleActionType == BattleActionType.EntityAction)
                    {
                        if (actionContext.TargetEntity == null
                            || actionContext.TargetEntity.IsDisposedFromBattle
                            || !actionContext.TargetEntity.IsAlive)
                        {
                            Logging.LogError("Action requires entity but it's not allowed one. Skipping perform.");
                            continue; //break;
                        }

                        if (_targetConditions != null && !_targetConditions.AllMet(
                                actionContext.BattleContext,
                                actionContext.ActionMaker,
                                actionContext.TargetEntity))
                            continue; //break;
                    }

                    if (AnimationBeforeAction != null)
                        await AnimationBeforeAction.Play(animationContext);

                    var performResult = await Action.ModifiedPerform(actionContext);
                    if (performResult.IsSuccessful) //Action Success
                    {
                        if (SuccessInstructionsEveryRepeat)
                            await ExecuteRecursiveInSequence(_instructionsOnActionSuccess, executionContext);
                        anyRepetitionSuccessed = true;
                    }
                    else //Action Fail
                    {
                        if (FailInstructionsEveryRepeat)
                            await ExecuteRecursiveInSequence(_instructionsOnActionFail, executionContext);
                    }

                    if (FollowInstructionsEveryRepeat)
                    {
                        await ExecuteRecursiveInSequence(_followingInstructions, executionContext);
                    }
                }

                if (anyRepetitionSuccessed && !SuccessInstructionsEveryRepeat)
                    await ExecuteRecursiveInSequence(_instructionsOnActionSuccess, executionContext);
                if (!anyRepetitionSuccessed && !FailInstructionsEveryRepeat)
                    await ExecuteRecursiveInSequence(_instructionsOnActionFail, executionContext);
                if (!FollowInstructionsEveryRepeat)
                    await ExecuteRecursiveInSequence(_followingInstructions, executionContext);
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

            #endregion
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

            AnimationBeforeAction = instruction.AnimationBeforeAction; //Not safe for changes (shared)
        }
    }
}