namespace OrderElimination.AbilitySystem
{
    public class EntityUsedAbilityFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public IActiveAbilityData UsedAbility { get; }

        public EntityUsedAbilityFireInfo(IBattleTrigger trigger, IActiveAbilityData usedAbility)
        {
            Trigger = trigger;
            UsedAbility = usedAbility;
        }
    }
}
