using Cysharp.Threading.Tasks;
using DG.Tweening;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class WalkAnimation : IAbilityAnimation
    {
        [HideInInspector, OdinSerialize]
        private float _time = 1;
        [HideInInspector, OdinSerialize]
        private float _speed = 1;

        [EnumToggleButtons]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget MovingEntity { get; set; } = AnimationTarget.Caster;

        #region Origin
        [TabGroup("Origin")]
        [ShowInInspector, OdinSerialize]
        public bool UseExecutionGroupForOrigin { get; set; } = false;

        [TabGroup("Origin")]
        [ShowIf("@!" + nameof(UseExecutionGroupForOrigin))]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget OriginTarget { get; set; } = AnimationTarget.Caster;

        [TabGroup("Origin")]
        [ShowIf("@" + nameof(UseExecutionGroupForOrigin))]
        [ShowInInspector, OdinSerialize]
        public int OriginCellGroup { get; set; }

        [TabGroup("Origin")]
        [ShowIf("@" + nameof(UseExecutionGroupForOrigin))]
        [ShowInInspector, OdinSerialize]
        public CellPriority OriginCellOrder { get; set; }
        #endregion

        #region Destination
        [TabGroup("Destination")]
        [ShowInInspector, OdinSerialize]
        public bool UseExecutionGroupForDestination { get; set; } = false;

        [TabGroup("Destination")]
        [ShowIf("@!" + nameof(UseExecutionGroupForDestination))]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget DestinationTarget { get; set; } = AnimationTarget.Target;

        [TabGroup("Destination")]
        [ShowIf("@" + nameof(UseExecutionGroupForDestination))]
        [ShowInInspector, OdinSerialize]
        public int DestinationCellGroup { get; set; }

        [TabGroup("Destination")]
        [ShowIf("@" + nameof(UseExecutionGroupForDestination))]
        [ShowInInspector, OdinSerialize]
        public CellPriority DestinationCellOrder { get; set; }
        #endregion

        [ShowInInspector, OdinSerialize, LabelText("Move By Constant")]
        public MoveByConstant MoveBy { get; set; } = MoveByConstant.Speed;

        [ShowInInspector, ShowIf("@MoveBy == MoveByConstant.Speed")]
        public float Speed
        {
            get => _speed;
            set
            {
                if (value < 0) value = 0;
                _speed = value;
            }
        }

        [ShowInInspector, ShowIf("@MoveBy == MoveByConstant.Time")]
        public float Time
        {
            get => _time;
            set
            {
                if (value < 0) value = 0;
                _time = value;
            }
        }

        [ShowInInspector, OdinSerialize]
        public Ease MoveEase { get; set; }

        public async UniTask Play(AnimationPlayContext context)
        {
            var movingTaget = MovingEntity switch
            {
                AnimationTarget.Target => context.TargetView,
                AnimationTarget.Caster => context.CasterView,
                _ => throw new NotImplementedException(),
            };

            var casterGamePos = context.CasterGamePosition;
            var targetGamePos = context.TargetGamePosition;

            Vector2Int from;
            if (UseExecutionGroupForOrigin)
            {
                var group = context.TargetedCellGroups.GetGroup(OriginCellGroup);
                from = OriginCellOrder.GetPositionByPriority(group, casterGamePos, targetGamePos);
            }
            else
            {
                from = OriginTarget switch
                {
                    AnimationTarget.Target => context.Target.Position,
                    AnimationTarget.Caster => context.Caster.Position,
                    _ => throw new NotImplementedException(),
                };
            }

            Vector2Int to;
            if (UseExecutionGroupForDestination)
            {
                var group = context.TargetedCellGroups.GetGroup(DestinationCellGroup);
                to = DestinationCellOrder.GetPositionByPriority(group, casterGamePos, targetGamePos);
            }
            else
            {
                to = DestinationTarget switch
                {
                    AnimationTarget.Target => context.TargetGamePosition.Value,
                    AnimationTarget.Caster => context.CasterGamePosition.Value,
                    _ => throw new NotImplementedException(),
                };
            }
            //Debug.Log($"Moving from {context.CasterGamePosition} to {context.TargetGamePosition}.");

            var realWorldStartPos = context.SceneContext.BattleMapView.GetCell(from.x, from.y).transform.position;
            var realWorldEndPos = context.SceneContext.BattleMapView.GetCell(to.x, to.y).transform.position;
            var offset = realWorldEndPos - realWorldStartPos;
            var time = MoveBy switch
            {
                MoveByConstant.Time => Time,
                MoveByConstant.Speed => offset.magnitude / Speed,
                _ => throw new NotImplementedException(),
            };
            movingTaget.transform.DOComplete();
            movingTaget.transform.position = realWorldStartPos;
            await movingTaget.transform
                .DOMove(realWorldEndPos, time)
                .SetEase(MoveEase)
                .AsyncWaitForCompletion();
        }
    }
}
