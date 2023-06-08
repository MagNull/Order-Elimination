using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;

namespace AI
{
    public interface IBehaviorTreeTask
    {
        public UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster);
    }
}