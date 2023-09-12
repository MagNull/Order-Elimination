namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectImmunityActionCallback : IBattleActionCallback
    {
        public ApplyEffectImmunityActionCallback(IEffectData blockedEffect)
        {
            BlockedEffect = blockedEffect;
        }

        public IEffectData BlockedEffect { get; }
    }
}
