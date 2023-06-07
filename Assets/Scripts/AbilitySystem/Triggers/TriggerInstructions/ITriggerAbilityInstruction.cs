using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerAbilityInstruction
    {
        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster);
    }
}
