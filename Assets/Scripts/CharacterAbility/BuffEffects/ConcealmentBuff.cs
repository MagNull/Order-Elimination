using OrderElimination.Battle;

namespace CharacterAbility.BuffEffects
{
    public class ConcealmentBuff : TickEffectBase
    {
        private readonly CharactersBank _characterBank;

        public ConcealmentBuff(int duration, bool isUnique, CharactersBank characterBank, ITickEffectView effectView) 
            : base(duration, effectView, isUnique)
        {
            _characterBank = characterBank;
        }

        protected override void OnStartTick(ITickTarget target)
        {
            if(target is not BattleCharacter battleCharacter)
                return;
            
            _characterBank.RemoveCharacter(battleCharacter);
        }

        public override bool Equals(ITickEffect tickEffect)
        {
            return tickEffect is ConcealmentBuff concealmentBuff && concealmentBuff.Duration == Duration;
        }

        protected override void OnEndTick(ITickTarget target)
        {
            if(target is not BattleCharacter battleCharacter)
                return;
            
            _characterBank.AddCharacter(battleCharacter);
        }
    }
}