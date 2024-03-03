using System;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;

namespace AI.Compositions
{
    [Serializable]
    public class Selector : CompositionTask
    {
        private BehaviorTreeTask[] _childrenTask;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            _childrenTask = GetChildrenTasks();
            foreach (var task in _childrenTask)
            {
                var result = await RecursivelyRunTask(task, blackboard);
                if (result)
                    return true;
                var caster = blackboard.Get<AbilitySystemActor>("caster");
                if (caster.IsDisposedFromBattle)
                    return false;
            }

            return false;
        }

        private async UniTask<bool> RecursivelyRunTask(BehaviorTreeTask task, Blackboard blackboard)
        {
            var result = await task.TryRun(blackboard);
            if (!result)
                return false;
        
            var nextTasks = task.GetChildrenTasks();
            if (nextTasks.Length == 0)
                return true;
            
            foreach (var nextTask in nextTasks)
            {
                result = await RecursivelyRunTask(nextTask, blackboard);
                if (!result)
                    return false;
            }

            return true;
        }
    }
}