﻿using Cysharp.Threading.Tasks;
using DG.Tweening;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class WalkAnimation : AwaitableAbilityAnimation
    {
        #region OdinVisuals
        private bool OriginAsCellGroup => OriginTarget == AnimationTarget.CellGroup;
        private bool DestinationAsCellGroup => DestinationTarget == AnimationTarget.CellGroup;
        #endregion

        [HideInInspector, OdinSerialize]
        private float _time = 1;
        [HideInInspector, OdinSerialize]
        private float _speed = 1;

        [EnumToggleButtons]
        [ShowInInspector, OdinSerialize]
        public ActionEntity MovingEntity { get; set; } = ActionEntity.Target;

        #region Origin
        [BoxGroup("Origin")]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget OriginTarget { get; set; } = AnimationTarget.Caster;

        [BoxGroup("Origin")]
        [EnableIf("@" + nameof(OriginAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public int OriginCellGroup { get; set; }

        [BoxGroup("Origin")]
        [EnableIf("@" + nameof(OriginAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public CellPriority OriginCellOrder { get; set; }
        #endregion

        #region Destination
        [BoxGroup("Destination")]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget DestinationTarget { get; set; } = AnimationTarget.Target;

        [BoxGroup("Destination")]
        [EnableIf("@" + nameof(DestinationAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public int DestinationCellGroup { get; set; }

        [BoxGroup("Destination")]
        [EnableIf("@" + nameof(DestinationAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public CellPriority DestinationCellOrder { get; set; }
        #endregion

        [BoxGroup("Movement Parameters")]
        [ShowInInspector, OdinSerialize, LabelText("Move By Constant")]
        public MoveByConstant MoveBy { get; set; } = MoveByConstant.Speed;

        [BoxGroup("Movement Parameters")]
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

        [BoxGroup("Movement Parameters")]
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

        [BoxGroup("Movement Parameters")]
        [ShowInInspector, OdinSerialize]
        public Ease MoveEase { get; set; }

        protected override async UniTask OnAnimationPlayRequest(AnimationPlayContext context, CancellationToken cancellationToken)
        {
            var movingTaget = MovingEntity switch
            {
                ActionEntity.Target => context.TargetView,
                ActionEntity.Caster => context.CasterView,
                _ => throw new NotImplementedException(),
            };

            var casterGamePos = context.CasterGamePosition;
            var targetGamePos = context.TargetGamePosition;

            Vector2Int from;
            Vector2Int to;
            switch (OriginTarget)
            {
                case AnimationTarget.Target:
                    from = context.Target.Position;
                    break;
                case AnimationTarget.Caster:
                    from = context.Caster.Position;
                    break;
                case AnimationTarget.CellGroup:
                    var positions = context.TargetedCellGroups.GetGroup(OriginCellGroup);
                    if (positions.Length == 0)
                        return;
                    from = OriginCellOrder.GetPositionByPriority(positions, casterGamePos, targetGamePos);
                    break;
                default:
                    throw new NotImplementedException();
            }

            switch (DestinationTarget)
            {
                case AnimationTarget.Target:
                    to = context.Target.Position;
                    break;
                case AnimationTarget.Caster:
                    to = context.Caster.Position;
                    break;
                case AnimationTarget.CellGroup:
                    var positions = context.TargetedCellGroups.GetGroup(DestinationCellGroup);
                    if (positions.Length == 0)
                        return;
                    to = DestinationCellOrder.GetPositionByPriority(positions, casterGamePos, targetGamePos);
                    break;
                default:
                    throw new NotImplementedException();
            }

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
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
        }
    }
}
