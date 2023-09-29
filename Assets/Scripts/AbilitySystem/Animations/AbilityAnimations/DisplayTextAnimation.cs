using Cysharp.Threading.Tasks;
using OrderElimination.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class DisplayTextAnimation : AwaitableAbilityAnimation
    {
        private bool DisplayAtCellGroup => DisplayNear == AnimationTarget.CellGroup;
        private bool NoOffsetsAssigned => 
            TextEmitterContext.Offset == Vector3.zero
            && TextEmitterContext.Origin == Vector3.zero;

        [BoxGroup("Positioning", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget DisplayNear { get; set; } = AnimationTarget.Target;

        [BoxGroup("Positioning")]
        [EnableIf("@" + nameof(DisplayAtCellGroup))]
        [ShowInInspector, OdinSerialize]
        public int DisplayCellGroup { get; set; }

        [BoxGroup("Positioning")]
        [EnableIf("@" + nameof(DisplayAtCellGroup))]
        [ShowInInspector, OdinSerialize]
        public bool PlaySimultaneously { get; set; } = true;

        [BoxGroup("Positioning")]
        [ShowInInspector, OdinSerialize]
        public Vector2 OffsetFromTarget { get; set; }

        [BoxGroup("Positioning")]
        [ShowInInspector, OdinSerialize]
        public Vector2 FloatingOffset { get; set; }

        [ValidateInput("@" + nameof(NoOffsetsAssigned), 
            "Origin and Offset will be defined by script. \nSet these in \"Positioning\" section!")]
        [HideLabel]
        [ShowInInspector, OdinSerialize]
        public TextEmitterContext TextEmitterContext { get; set; } = TextEmitterContext.Default;

        protected override async UniTask OnAnimationPlayRequest(AnimationPlayContext context, CancellationToken cancellationToken)
        {
            var targetPositions = DisplayNear switch
            {
                AnimationTarget.Target => new[] { context.TargetGamePosition.Value },
                AnimationTarget.Caster => new[] { context.CasterGamePosition.Value },
                AnimationTarget.CellGroup => context.TargetedCellGroups.GetGroup(DisplayCellGroup),
                _ => throw new NotImplementedException()
            };
            var textTasks = new List<UniTask>();
            foreach (var position in targetPositions)
            {
                var textStartPosition = position + OffsetFromTarget;
                var textEndPosition = position + OffsetFromTarget + FloatingOffset;
                var realWorldStartPosition = context.SceneContext.BattleMapView.GameToWorldPosition(textStartPosition);
                var realWorldEndPosition = context.SceneContext.BattleMapView.GameToWorldPosition(textEndPosition);
                var offset = realWorldEndPosition - realWorldStartPosition;
                var modifiedContext = TextEmitterContext;
                modifiedContext.Origin = realWorldStartPosition;
                modifiedContext.Offset = offset;
                var task = context.SceneContext.TextEmitter
                        .Emit(modifiedContext)
                        .AttachExternalCancellation(cancellationToken);
                if (PlaySimultaneously)
                {
                    textTasks.Add(task);
                }
                else
                {
                    await task;
                }
            }
            if (PlaySimultaneously)
                await UniTask.WhenAll(textTasks);
        }
    }
}
