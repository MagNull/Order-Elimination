namespace OrderElimination.AbilitySystem
{
    public class EffectsChangedFireInfo : ITriggerFireInfo
    {
        public enum EffectChangeType
        {
            EffectAdded,
            EffectRemoved
        }

        public EffectsChangedFireInfo(
            IBattleTrigger trigger, BattleEffect effect, EffectChangeType changeType)
        {
            Trigger = trigger;
            Effect = effect;
            ChangeType = changeType;
        }

        public IBattleTrigger Trigger { get; }

        public BattleEffect Effect { get; }

        public EffectChangeType ChangeType { get; }
    }
}
