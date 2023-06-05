namespace OrderElimination.AbilitySystem
{
    public interface ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
    }

    public class EmptyTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }

        public EmptyTriggerFireInfo(IBattleTrigger trigger)
        {
            Trigger = trigger;
        }
    }
}
