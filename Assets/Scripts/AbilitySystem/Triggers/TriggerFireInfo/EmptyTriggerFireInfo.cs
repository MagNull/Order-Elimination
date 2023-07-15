namespace OrderElimination.AbilitySystem
{
    public class EmptyTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }

        public EmptyTriggerFireInfo(IBattleTrigger trigger)
        {
            Trigger = trigger;
        }
    }
}
