using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;

namespace OrderElimination.Battle
{
    public static class BattleUnloader
    {
        public static GameCharacter[] UnloadCharacters(
            IBattleContext battleContext, 
            GameCharacter[] characters)
        {
            var entitiesBank = battleContext.EntitiesBank;
            foreach (var character in characters)
            {
                // var battleEntity = entitiesBank.GetEntityByBasedCharacter(character);
                // character.CurrentHealth = battleEntity.BattleStats.Health;
                // if (!battleEntity.IsDisposedFromBattle)
                //     battleEntity.DisposeFromBattle();
            }
            return characters;
        }
    }
}
