using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class MoveAction : BattleAction<MoveAction>, IUtilizeCellGroupsAction
    {
        [ShowInInspector, OdinSerialize]
        public int DestinationCellGroup { get; private set; }

        //public List<ICellCondition> DestinationCellConditions { get; private set; }

        [ShowInInspector, OdinSerialize]
        public CellPriority CellPriority { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool UsePath { get; private set; }

        [ShowInInspector, OdinSerialize, ShowIf("@" + nameof(UsePath))]
        public List<ICellCondition> PathConditions { get; private set; } = new();
        //LimitedByCasterMovementDistance?

        [ShowInInspector, OdinSerialize]
        public bool ForceMove { get; set; } = false; //if entity can not move

        //[GUIColor(0, 1, 1)]
        //[ShowInInspector, OdinSerialize]
        //private bool _overrideDefaultAnimations { get; set; } = false;

        [GUIColor(0, 1, 1)]
        //[ShowIf("@" + nameof(_overrideDefaultAnimations))]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation MoveAnimation { get; private set; }

        [GUIColor(0, 1, 1)]
        //[ShowIf("@" + nameof(_overrideDefaultAnimations))]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation MoveFailedAnimation { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public int[] UtilizedCellGroups => new[] { DestinationCellGroup };

        public override IBattleAction Clone()
        {
            var clone = new MoveAction();
            clone.DestinationCellGroup = DestinationCellGroup;
            clone.CellPriority = CellPriority;
            clone.UsePath = UsePath;
            clone.PathConditions = PathConditions.DeepClone();
            clone.ForceMove = ForceMove;
            clone.MoveAnimation = MoveAnimation;
            clone.MoveFailedAnimation = MoveFailedAnimation;
            return clone;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var target = useContext.ActionTarget;
            var failResult = new SimplePerformResult(this, useContext, false);
            var cellGroups = useContext.CellTargetGroups;
            if (!cellGroups.ContainsGroup(DestinationCellGroup)
                || cellGroups.GetGroup(DestinationCellGroup).Length == 0)
                return failResult;
            if (!target.CanMove && !ForceMove)
                return failResult;
            var battleContext = useContext.BattleContext;
            var casterPos = useContext.ActionMaker.Position;
            Vector2Int? targetPos = null;
            if (target != null)
                targetPos = target.Position;
            var destination = CellPriority.GetPositionByPriority(
                cellGroups.GetGroup(DestinationCellGroup), casterPos, targetPos);//destination conditions?
            var movingEntity = target;

            //var moveAnimation = _overrideDefaultAnimations
            //    ? MoveAnimation
            //    : useContext.AnimationSceneContext.DefaultAnimations.GetDefaultAnimation(DefaultAnimation.Walk);

            if (UsePath)
            {
                var success = false;
                if (useContext.BattleContext.BattleMap
                    .PathExists(movingEntity.Position, destination, IsPathPositionAvailable, out var path))
                {
                    var currentPoint = movingEntity.Position;
                    var finishedSuccessfully = true;
                    for (var i = 0; i < path.Length; i++)
                    {
                        if (!IsPathPositionAvailable(path[i]))
                            return failResult;
                        if (!useContext.ActionTarget.CanMove && !ForceMove)
                            return failResult;
                        var fakeGroupsContainer = new CellGroupsContainer(new Dictionary<int, Vector2Int[]>
                        {
                            { DestinationCellGroup, new[] { path[i] } }
                        });
                        var pathAnimContext = new AnimationPlayContext(
                            useContext.AnimationSceneContext,
                            fakeGroupsContainer,
                            useContext.ActionMaker,
                            useContext.ActionTarget,
                            path[i]);
                        if (movingEntity.CanMove )
                        if (MoveAnimation != null)
                            await MoveAnimation.Play(pathAnimContext);
                        if (!movingEntity.Move(path[i]))
                        {
                            finishedSuccessfully = false;
                            if (MoveFailedAnimation != null)
                                await MoveFailedAnimation.Play(pathAnimContext);
                            break;
                        }
                        currentPoint = path[i];
                        //Logging.Log($"Path cell {i}: {path[i]}" , Colorize.Orange);
                    }
                    success = finishedSuccessfully;
                }
                return new SimplePerformResult(this, useContext, success);
            }
            else
            {
                var animationContext = new AnimationPlayContext(
                    useContext.AnimationSceneContext,
                    useContext.CellTargetGroups,
                    useContext.ActionMaker,
                    useContext.ActionTarget,
                    targetPos);
                if (MoveAnimation != null)
                    await MoveAnimation.Play(animationContext);
                var moved = movingEntity.Move(destination, ForceMove);
                if (!moved)
                {
                    await MoveFailedAnimation.Play(animationContext);
                }
                return new SimplePerformResult(this, useContext, moved);
            }

            bool IsPathPositionAvailable(Vector2Int position)
            {
                return PathConditions == null 
                    || PathConditions.All(c => c.IsConditionMet(battleContext, movingEntity, position));
            }
        }
    }
}
