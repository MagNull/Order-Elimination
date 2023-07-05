using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.MacroGame
{
    public static class GameCharactersFactory
    {
        public static GameCharacter CreateGameEntity(IGameCharacterTemplate characterTemplate)
        {
            var activeAbilities = characterTemplate.GetActiveAbilities().Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = characterTemplate.GetPassiveAbilities().Select(a => AbilityFactory.CreatePassiveAbility(a));
            return new GameCharacter(characterTemplate, activeAbilities, passiveAbilities);
        }

        //public static GameCharacter RestoreGameCharacter(IGameCharacterTemplate characterTemplate)
        //{

        //}

        public static IEnumerable<GameCharacter> CreateGameEntities(IEnumerable<IGameCharacterTemplate> characterTempaltes)
            => characterTempaltes.Select(gameEntity => CreateGameEntity(gameEntity));
    }
}
