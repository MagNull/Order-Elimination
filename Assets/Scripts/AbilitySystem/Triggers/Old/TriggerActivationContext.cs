namespace OrderElimination.AbilitySystem
{
    public readonly struct TriggerActivationContext
    {
        public readonly ITriggerSetupInfo SetupInfo;
        public readonly IBattleContext BattleContext;
        public readonly AbilitySystemActor TrackingEntity;

        public TriggerActivationContext(
            ITriggerSetupInfo setupInfo, 
            IBattleContext battleContext, 
            AbilitySystemActor trackingEntity = null)
        {
            SetupInfo = setupInfo;
            BattleContext = battleContext;
            TrackingEntity = trackingEntity;
        }
    }
}
