using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.MacroGame
{
    public static class GameCharactersFactory
    {
        public static GameCharacter CreateGameCharacter(IGameCharacterTemplate characterTemplate)
        {
            var activeAbilities = characterTemplate.GetActiveAbilities()
                .Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = characterTemplate.GetPassiveAbilities()
                .Select(a => AbilityFactory.CreatePassiveAbility(a));
            var character = new GameCharacter(characterTemplate, activeAbilities, passiveAbilities);
            character.CurrentHealth = character.CharacterStats.MaxHealth;
            return character;
        }

        //public static GameCharacter RestoreGameCharacter(IGameCharacterTemplate characterTemplate)
        //{

        //}

        public static IEnumerable<GameCharacter> CreateGameEntities(
            IEnumerable<IGameCharacterTemplate> characterTempaltes)
            => characterTempaltes.Select(gameEntity => CreateGameCharacter(gameEntity));
    }
}