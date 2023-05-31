using System;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace AI
{
    [Serializable]
    public class Sequence : IBehaviorTreeTask
    {
        [SerializeField]
        private IBehaviorTreeTask[] _childrenTask;

        public bool Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            foreach (var task in _childrenTask)
            {
                if (!task.Run(battleContext, caster))
                    return false;
            }

            return true;
        }
    }
}