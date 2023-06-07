using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerInstruction
    {
        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster);
    }
}
