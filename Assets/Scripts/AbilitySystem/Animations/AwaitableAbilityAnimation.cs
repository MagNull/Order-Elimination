using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

namespace OrderElimination.AbilitySystem.Animations
{
    public abstract class AwaitableAbilityAnimation : IAbilityAnimation
    {
        public async UniTask Play(AnimationPlayContext context)
        {
            var tokenSource = new CancellationTokenSource();
            context.SceneContext.AllAnimationsStopRequested += OnCancellationRequested;
            //SceneManager.sceneUnloaded += OnSceneUnloaded;
            await OnAnimationPlayRequest(context, tokenSource.Token);//.SuppressCancellationThrow();
            //tokenSource.Dispose();

            void OnSceneUnloaded(Scene scene)
            {
                if (scene == context.SceneContext.CurrentScene)
                    CancelAnimation();
            }

            void OnCancellationRequested(AnimationSceneContext context)
            {
                CancelAnimation();
            }

            void CancelAnimation()
            {
                tokenSource.Cancel();
                Logging.Log("Animation Cancelled");
            }
        }

        protected abstract UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken);
    }
}
