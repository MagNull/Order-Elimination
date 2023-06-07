﻿using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;

namespace AI.Utils
{
    public static class AIBankExtension
    {
        public static AbilitySystemActor[] GetEnemiesByDistance(this IReadOnlyEntitiesBank entitiesBank,
            IBattleContext battleContext, AbilitySystemActor caster)
        {
            var enemies = entitiesBank.GetEntities(BattleSide.Allies)
                .Union(entitiesBank.GetEntities(BattleSide.Player))
                .Where(ent => !ent.StatusHolder.HasStatus(BattleStatus.Invisible));
            return enemies
                .OrderBy(e => battleContext.BattleMap.GetGameDistanceBetween(e.Position, caster.Position))
                .ToArray();
        }

        public static AbilitySystemActor[] GetEnemiesByValue(this IReadOnlyEntitiesBank entitiesBank)
        {
            var enemies = entitiesBank.GetEntities(BattleSide.Allies)
                .Union(entitiesBank.GetEntities(BattleSide.Player))
                .Where(ent => !ent.StatusHolder.HasStatus(BattleStatus.Invisible));
            return enemies
                .OrderByDescending(e => entitiesBank.GetBattleCharacterData(e).CharacterData.CostValue)
                .ToArray();
        }
    }
}