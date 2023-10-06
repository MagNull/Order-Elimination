namespace OrderElimination.AbilitySystem
{
    public interface IContextTriggerSetup : ITriggerSetup
    {
        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor activator);
    }
}
