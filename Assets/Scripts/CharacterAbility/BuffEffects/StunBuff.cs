namespace CharacterAbility.BuffEffects
{
    public class StunBuff : TickEffectBase
    {
        private int _startDuration;
        
        public StunBuff(int duration, ITickEffectView effectView, bool isUnique) : base(duration, effectView, isUnique)
        {
            _startDuration = duration;
        }

        public void Apply(IActor actor)
        {
            actor.ClearActions();
        }

        public override bool Equals(ITickEffect tickEffect)
        {
            return tickEffect is StunBuff stunBuff && stunBuff._startDuration == _startDuration;
        }
    }
}