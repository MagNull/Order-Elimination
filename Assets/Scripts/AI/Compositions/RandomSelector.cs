using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class RandomSelector : IBehaviorTreeTask
    {
        [SerializeReference]
        private IBehaviorTreeTask[] _childrenTask;

        public async UniTask<bool> Run(Blackboard blackboard)
        {
            _childrenTask = _childrenTask.OrderBy(x => Random.Range(0, _childrenTask.Length)).ToArray();
            foreach (var task in _childrenTask)
            {
                var result = await task.Run(blackboard);
                if (result)
                    return true;
            }

            return false;
        }
    }
}