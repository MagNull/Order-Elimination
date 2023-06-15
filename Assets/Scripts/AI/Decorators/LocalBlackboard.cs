using AI.Compositions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AI.Decorators
{
    public class LocalBlackboard : BehaviorTreeTask
    {
        [Output]
        [SerializeField]
        private TaskPort ChildrenPort;
        
        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            var new_bb = new Blackboard(blackboard);
            return await GetChildrenTasks()[0].Run(new_bb);
        }
    }
}