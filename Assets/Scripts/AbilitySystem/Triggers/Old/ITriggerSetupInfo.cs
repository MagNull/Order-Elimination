namespace OrderElimination.AbilitySystem
{
    public interface ITriggerSetupInfo
    {
        public void SubscribeTrigger(BattleTrigger battleTrigger, TriggerActivationContext context);
    }
}
