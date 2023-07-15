namespace OrderElimination.AbilitySystem
{
    public class EntityDamagedTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public DealtDamageInfo DamageInfo { get; }

        public EntityDamagedTriggerFireInfo(IBattleTrigger trigger, DealtDamageInfo damageInfo)
        {
            Trigger = trigger;
            DamageInfo = damageInfo;
        }

    }
}
