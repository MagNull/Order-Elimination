namespace OrderElimination.AbilitySystem
{
    public class DamageTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public DealtDamageInfo DamageInfo { get; }

        public DamageTriggerFireInfo(IBattleTrigger trigger, DealtDamageInfo damageInfo)
        {
            Trigger = trigger;
            DamageInfo = damageInfo;
        }

    }
}
