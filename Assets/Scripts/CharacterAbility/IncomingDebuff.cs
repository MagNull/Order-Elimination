namespace CharacterAbility
{
    public enum IncomingDebuffType
    {
        Attack,
        Accuracy
    }
    
    public class IncomingDebuff : ITickEffect
    {
        private readonly ITickTarget _tickTarget;
        private readonly IncomingDebuffType _incomingDebuffType;
        private readonly int _modificator;
        private int _duration;

        public IncomingDebuffType DebuffType => _incomingDebuffType;

        public IncomingDebuff(ITickTarget tickTarget, IncomingDebuffType incomingDebuffType, int duration, int modificator)
        {
            _tickTarget = tickTarget;
            _incomingDebuffType = incomingDebuffType;
            _duration = duration;
            _modificator = modificator;
        }

        public int GetModifiedValue(int value) => value + _modificator;
        
        public void Tick()
        {
            _duration--;
            if(_duration <= 0)
                _tickTarget.RemoveTickEffect(this);
        }
    }
}