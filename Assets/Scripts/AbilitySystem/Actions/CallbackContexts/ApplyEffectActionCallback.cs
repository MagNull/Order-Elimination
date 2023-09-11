namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectActionCallback : IBattleActionCallback
    {
        public ApplyEffectActionCallback(BattleEffect effect)
        {
            Effect = effect;
        }

        public BattleEffect Effect { get; }//Deactivated effect
    }
}
