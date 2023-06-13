using System;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI
{
    [Serializable]
    public class Selector : IBehaviorTreeTask
    {
        [SerializeReference]
        private IBehaviorTreeTask[] _childrenTask;
        
        public async UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            foreach (var task in _childrenTask)
            {
                var result = await task.Run(battleContext, caster);
                if (result)
                    return true;
            }

            return false;
        }
    }
}