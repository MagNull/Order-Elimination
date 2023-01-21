using OrderElimination.Battle;

namespace CharacterAbility.BuffEffects
{
    public class ConcealmentBuff : TickEffectBase
    {
        private readonly CharactersBank _characterBank;

        public ConcealmentBuff(int duration, CharactersBank characterBank, ITickEffectView effectView) 
            : base(duration, effectView)
        {
            _characterBank = characterBank;
        }

        protected override void OnStartTick(ITickTarget target)
        {
            if(target is not BattleCharacter battleCharacter)
                return;
            
            _characterBank.RemoveCharacter(battleCharacter);
        }

        protected override void OnEndTick(ITickTarget target)
        {
            if(target is not BattleCharacter battleCharacter)
                return;
            
            _characterBank.AddCharacter(battleCharacter);
        }
    }
}