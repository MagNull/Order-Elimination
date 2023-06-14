using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AI.Decorators
{
    public abstract class Decorator : IBehaviorTreeTask
    {
        [SerializeReference]
        protected IBehaviorTreeTask _childrenTask;

        public abstract UniTask<bool> Run(Blackboard blackboard);
    }
}