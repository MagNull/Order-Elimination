using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class MoveAction : BattleAction<MoveAction>, IUtilizeCellGroupsAction
    {
        [ShowInInspector, OdinSerialize]
        public ActionEntity PerformEntity { get; private set; } = ActionEntity.Caster;

        [ShowInInspector, OdinSerialize]
        public int DestinationCellGroup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public CellPriority CellPriority { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool UsePath { get; private set; }

        [ShowInInspector, OdinSerialize, ShowIf("@" + nameof(UsePath))]
        public List<ICellCondition> PathConditions { get; private set; } = new();
        //LimitedByCasterMovementDistance?

        [ShowInInspector, OdinSerialize]
        public bool ForceMove { get; set; } = false; //if entity can not move

        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation MoveAnimation { get; private set; }

        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation MoveFailedAnimation { get; private set; }
        //MoveAnimation
        //MoveFailAnimation
        //OverrideMoveAnimation (or use default)

        public override ActionRequires ActionRequires
        {
            get
            {
                return PerformEntity switch
                {
                    ActionEntity.Caster => ActionRequires.Nothing,
                    ActionEntity.Target => ActionRequires.Entity,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public IEnumerable<int> UtilizingCellGroups => new[] { DestinationCellGroup };

        public override IBattleAction Clone()
        {
            var clone = new MoveAction();
            clone.PerformEntity = PerformEntity;
            clone.DestinationCellGroup = DestinationCellGroup;
            clone.CellPriority = CellPriority;
            clone.UsePath = UsePath;
            clone.PathConditions = PathConditions.Clone();
            clone.ForceMove = ForceMove;
            clone.MoveAnimation = MoveAnimation;
            clone.MoveFailedAnimation = MoveFailedAnimation;
            return clone;
        }

        public int GetUtilizedCellsAmount(int group) => group == DestinationCellGroup ? 1 : 0;

        protected override async UniTask<bool> Perform(ActionContext useContext)
        {
            var cellGroups = useContext.TargetCellGroups;
            if (!cellGroups.ContainsGroup(DestinationCellGroup)
                || cellGroups.GetGroup(DestinationCellGroup).Length == 0)
                return false;

            var battleContext = useContext.BattleContext;
            var casterPos = useContext.ActionMaker.Position;
            var targetPos = useContext.ActionTargetInitialPosition;
            var destination = CellPriority.GetPositionByPriority(
                cellGroups.GetGroup(DestinationCellGroup), casterPos, targetPos);
            var movingEntity = useContext.GetActionEntity(PerformEntity);
            //path
            if (UsePath)
            {
                if (useContext.BattleContext.BattleMap
                    .PathExists(movingEntity.Position, destination, IsPositionAvailable, out var path))
                {
                    var currentPoint = movingEntity.Position;
                    var finishedSuccessfully = true;
                    for (var i = 0; i < path.Length; i++)
                    {
                        var pathAnimContext = new AnimationPlayContext(
                            useContext.AnimationSceneContext,
                            useContext.TargetCellGroups,
                            movingEntity.Position,
                            path[i],
                            useContext.ActionMaker,
                            useContext.ActionTarget);
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
                        Debug.Log($"Path cell {i}: {path[i]}" % Colorize.Orange);
                    }
                    return finishedSuccessfully;
                }
                else
                    return false;
            }
            //path
            var animationContext = new AnimationPlayContext(
                useContext.AnimationSceneContext,
                useContext.TargetCellGroups,
                useContext.ActionMaker.Position,
                useContext.ActionTargetInitialPosition,
                useContext.ActionMaker,
                useContext.ActionTarget);
            if (MoveAnimation != null)
                await MoveAnimation.Play(animationContext);
            var moved = movingEntity.Move(destination, ForceMove);
            if (!moved)
            {
                await MoveFailedAnimation.Play(animationContext);
            }
            return moved;

            bool IsPositionAvailable(Vector2Int position)
            {
                return PathConditions == null 
                    || PathConditions.All(c => c.IsConditionMet(battleContext, movingEntity, position));
            }
        }
    }
}
