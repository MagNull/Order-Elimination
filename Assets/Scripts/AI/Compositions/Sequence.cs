using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AI.Compositions
{
    [Serializable]
    public class Sequence : BehaviorTreeTask
    {
        [Output]
        [SerializeField]
        private TaskPort ChildrenPort;
        
        private BehaviorTreeTask[] _childrenTask;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            _childrenTask = GetChildrenTasks();
            foreach (var task in _childrenTask)
            {
                var result = await task.TryRun(blackboard);
                if (!result)
                    return false;
            }

            return true;
        }
    }
}