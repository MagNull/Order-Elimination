using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AbilitySystem.PrototypeHelpers
{
    public abstract class BattlePlayer
    {
        public IReadOnlyList<AbilitySystemActor> ControllingEntities;

        public async UniTask OnTurnStarted(IBattleContext battleContext)
        {

        }
    }
}
