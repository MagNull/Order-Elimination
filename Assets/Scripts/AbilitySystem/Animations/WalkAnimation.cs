using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class WalkAnimation : IAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public float MoveTime { get; private set; }

        public bool IsFinished { get; private set; }

        public event Action<IAbilityAnimation> Finished;

        public async UniTask Play(AnimationPlayContext context)
        {
            //Debug.Log($"Moving from {context.CasterGamePosition} to {context.TargetGamePosition}.");
            IsFinished = false;
            var from = context.CasterGamePosition.Value;
            var to = context.TargetGamePosition.Value;
            var realWorldStartPos = context.SceneContext.BattleMapView.GetCell(from.x, from.y).transform.position;
            var realWorldEndPos = context.SceneContext.BattleMapView.GetCell(to.x, to.y).transform.position;
            context.CasterView.transform.position = realWorldStartPos;
            context.CasterView.transform.DOComplete();
            await context.CasterView.transform.DOMove(realWorldEndPos, MoveTime).AsyncWaitForCompletion();//.OnComplete(OnMoveComplete);
            IsFinished = true;
            //while (!IsFinished)
            //{
            //    await UniTask.Yield();
            //}
            Finished?.Invoke(this);
        }
    }
}
