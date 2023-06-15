using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationCondition : IAbilityAnimation
    {
        //Conditions
        //Animation

        public async UniTask Play(AnimationPlayContext context)
        {
            Logging.LogException( new NotImplementedException());
        }
    }
}
