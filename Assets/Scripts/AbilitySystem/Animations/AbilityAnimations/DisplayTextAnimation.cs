using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class DisplayTextAnimation : IAbilityAnimation
    {
        [ShowInInspector, OdinSerialize, MultiLineProperty]
        public string Text { get; set; } = "Sample Text";

        [ShowInInspector, OdinSerialize]
        public Color Color { get; set; } = Color.white;

        [ShowInInspector, OdinSerialize]
        public AnimationTarget DisplayNear { get; set; } = AnimationTarget.Target;

        [ShowInInspector, OdinSerialize]
        public float Duration { get; set; } = 1f;

        [ShowInInspector, OdinSerialize]
        public Vector2 OffsetFromTarget { get; set; }

        [ShowInInspector, OdinSerialize]
        public Vector2 FloatingOffset { get; set; }

        public async UniTask Play(AnimationPlayContext context)
        {
            var targetPos = DisplayNear switch
            {
                AnimationTarget.Target => context.TargetGamePosition,
                AnimationTarget.Caster => context.CasterGamePosition,
                _ => throw new System.NotImplementedException()
            };
            if (!targetPos.HasValue)
                return;
            var textStartPosition = targetPos + OffsetFromTarget;
            var textEndPosition = targetPos + OffsetFromTarget + FloatingOffset;
            var realWorldStartPosition = context.SceneContext.BattleMapView.GameToWorldPosition(textStartPosition.Value);
            var realWorldEndPosition = context.SceneContext.BattleMapView.GameToWorldPosition(textEndPosition.Value);
            var offset = realWorldEndPosition - realWorldStartPosition;
            await context.SceneContext.TextEmitter.Emit(Text, Color, realWorldStartPosition, offset, Duration, -1);
        }
    }
}
