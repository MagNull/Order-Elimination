﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public static class GameCharactersFactory
    {
        public static GameCharacter CreateGameEntity(IBattleCharacterData entityInfo)
        {
            var activeAbilities = entityInfo.GetActiveAbilities().Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = entityInfo.GetPassiveAbilities().Select(a => AbilityFactory.CreatePassiveAbility(a));
            return new GameCharacter(entityInfo, activeAbilities, passiveAbilities);
        }

        public static IEnumerable<GameCharacter> CreateGameEntities(IEnumerable<IBattleCharacterData> entityInfos)
            => entityInfos.Select(gameEntity => CreateGameEntity(gameEntity));
    }
}