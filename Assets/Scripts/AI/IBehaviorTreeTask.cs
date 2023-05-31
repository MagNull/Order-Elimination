using OrderElimination.AbilitySystem;

namespace AI
{
    public interface IBehaviorTreeTask
    {
        public bool Run(IBattleContext battleContext, AbilitySystemActor caster);
    }
}