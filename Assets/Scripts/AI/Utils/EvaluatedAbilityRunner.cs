using OrderElimination.AbilitySystem;

namespace AI.Utils
{
    public class EvaluatedAbilityRunner
    {
        public EvaluatedAbilityRunner(ActiveAbilityRunner abilityRunner, AbilityImpact abilityImpact)
        {
            AbilityRunner = abilityRunner;
            AbilityImpact = abilityImpact;
        }

        public ActiveAbilityRunner AbilityRunner { get; }
        public AbilityImpact AbilityImpact { get; }
    }
}
