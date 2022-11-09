using OrderElimination;

namespace CharacterAbility.BuffEffects
{
    public class EvasionStatsBuff : IStatsBuffEffect
    {
        private readonly IBattleObject _target;
        private readonly int _value;
        private int _duration;

        public EvasionStatsBuff(IBattleObject target, int value, int duration)
        {
            _target = target;
            _value = value;
            _duration = duration;
        }

        public BattleStats Apply()
        {
            var newStats = new BattleStats(_target.Stats)
            {
                Evasion = _target.Stats.Evasion + _value
            };
            return newStats;
        }

        public void Tick()
        {
            _duration--;
            if (_duration <= 0)
                _target.RemoveTickEffect(this);
        }

        public BattleStats Remove()
        {
            var newStats = new BattleStats(_target.Stats)
            {
                Evasion = _target.Stats.Evasion - _value
            };
            return newStats;
        }
    }
}