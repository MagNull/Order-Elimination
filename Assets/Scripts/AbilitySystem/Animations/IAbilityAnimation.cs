using Cysharp.Threading.Tasks;
using System;

namespace OrderElimination.AbilitySystem.Animations
{
    //TODO-Animation Change to abstract class
    //Add option to await
    public interface IAbilityAnimation
    {
        public UniTask Play(AnimationPlayContext context);
    }
}
