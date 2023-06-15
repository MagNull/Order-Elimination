using Cysharp.Threading.Tasks;

namespace AI.Decorators
{
    public class LocalBlackboard : Decorator
    {
        public override async UniTask<bool> Run(Blackboard blackboard)
        {
            var new_bb = new Blackboard(blackboard);
            return await _childrenTask.Run(new_bb);
        }
    }
}