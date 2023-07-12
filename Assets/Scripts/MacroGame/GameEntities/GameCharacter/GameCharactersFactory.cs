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
            return RestoreGameCharacter(template, baseStats, baseStats.MaxHealth);
        }

        public static GameCharacter RestoreGameCharacter(
            IGameCharacterTemplate template, 
            IReadOnlyGameCharacterStats specifiedStats,
            float health)
        {
            var activeAbilities = template.GetActiveAbilities()
                .Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = template.GetPassiveAbilities()
                .Select(a => AbilityFactory.CreatePassiveAbility(a));
            var character = new GameCharacter(template, activeAbilities, passiveAbilities, specifiedStats);
            character.CurrentHealth = health;
            return character;
        }

        public static IEnumerable<GameCharacter> CreateGameCharacters(
            IEnumerable<IGameCharacterTemplate> characterTempaltes)
            => characterTempaltes.Select(gameEntity => CreateGameCharacter(gameEntity));
    }
}