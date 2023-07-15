using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Compositions
{
    public class RandomSelector : BehaviorTreeTask
    {
        [Output]
        [SerializeField]
        private TaskPort ChildrenPort;
        
        private BehaviorTreeTask[] _childrenTask;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            _childrenTask = GetChildrenTasks();
            _childrenTask = _childrenTask.OrderBy(x => Random.Range(0, _childrenTask.Length)).ToArray();
            foreach (var task in _childrenTask)
            {
                var result = await task.TryRun(blackboard);
                if (result)
                    return true;
            }

            return false;
        }
    }
}