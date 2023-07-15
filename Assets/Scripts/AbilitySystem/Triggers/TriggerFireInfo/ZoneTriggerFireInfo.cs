namespace OrderElimination.AbilitySystem
{
    public class ZoneTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public AbilitySystemActor[] CurrentEntities { get; }
        public AbilitySystemActor[] NewEntities { get; }
        public AbilitySystemActor[] DisappearedEntities { get; }

        public ZoneTriggerFireInfo(
            IBattleTrigger trigger,
            AbilitySystemActor[] currentEntities, 
            AbilitySystemActor[] newEntities, 
            AbilitySystemActor[] disappearedEntities)
        {
            Trigger = trigger;
            CurrentEntities = currentEntities;
            NewEntities = newEntities;
            DisappearedEntities = disappearedEntities;
        }
    }
}
