using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.MacroGame
{
    public static class GameCharactersFactory
    {
        public static GameCharacter CreateGameCharacter(IGameCharacterTemplate template)
        {
            var baseStats = template.GetBaseBattleStats();
            return CreateGameCharacter(template, baseStats);
        }

        public static GameCharacter CreateGameCharacter(
            IGameCharacterTemplate template, 
            IReadOnlyGameCharacterStats specifiedStats)
        {
            return CreateGameCharacter(
                template,
                specifiedStats,
                template.GetActiveAbilities(),
                template.GetPassiveAbilities());
        }

        public static GameCharacter CreateGameCharacter(
            IGameCharacterTemplate template,
            IReadOnlyGameCharacterStats specifiedStats,
            ActiveAbilityBuilder[] specifiedActiveAbilities,
            PassiveAbilityBuilder[] specifiedPassiveAbilities)
        {
            var activeAbilities = specifiedActiveAbilities
                .Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = specifiedPassiveAbilities
                .Select(a => AbilityFactory.CreatePassiveAbility(a));
            var character = new GameCharacter(template, activeAbilities, passiveAbilities, specifiedStats);
            character.CurrentHealth = character.CharacterStats.MaxHealth;
            return character;
        }

        public static IEnumerable<GameCharacter> CreateGameCharacters(
            IEnumerable<IGameCharacterTemplate> characterTempaltes)
            => characterTempaltes.Select(gameEntity => CreateGameCharacter(gameEntity));
    }
}