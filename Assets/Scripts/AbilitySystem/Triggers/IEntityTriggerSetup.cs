namespace OrderElimination.AbilitySystem
{
    public interface IEntityTriggerSetup : ITriggerSetup
    {
        public IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor activator, AbilitySystemActor trackingEntity);
    }
}
