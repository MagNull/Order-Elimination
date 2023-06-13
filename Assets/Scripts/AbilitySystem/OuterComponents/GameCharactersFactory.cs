using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.MetaGame
{
    public static class GameCharactersFactory
    {
        public static GameCharacter CreateGameEntity(IGameCharacterData entityInfo)
        {
            var activeAbilities = entityInfo.GetActiveAbilities().Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = entityInfo.GetPassiveAbilities().Select(a => AbilityFactory.CreatePassiveAbility(a));
            return new GameCharacter(entityInfo, activeAbilities, passiveAbilities);
        }

        public static IEnumerable<GameCharacter> CreateGameEntities(IEnumerable<IGameCharacterData> entityInfos)
            => entityInfos.Select(gameEntity => CreateGameEntity(gameEntity));
    }
}
