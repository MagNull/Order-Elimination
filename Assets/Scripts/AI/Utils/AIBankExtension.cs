using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;

namespace AI.Utils
{
    public static class AIBankExtension
    {
        public static AbilitySystemActor[] GetTargetsByDistance(this IReadOnlyEntitiesBank entitiesBank,
            IBattleContext battleContext, AbilitySystemActor caster, BattleRelationship relationship)
        {
            var enemies = entitiesBank.GetActiveEntities()
                .Where(en =>
                    battleContext.GetRelationship(caster.BattleSide, en.BattleSide) == relationship)
                .Where(ent => !ent.StatusHolder.HasStatus(BattleStatus.Invisible));
            return enemies
                .OrderBy(e => battleContext.BattleMap.GetGameDistanceBetween(e.Position, caster.Position))
                .ToArray();
        }

        public static AbilitySystemActor[] GetTargetsByValue(this IReadOnlyEntitiesBank entitiesBank,
            IBattleContext battleContext, AbilitySystemActor caster, BattleRelationship relationship)
        {
            var enemies = entitiesBank.GetActiveEntities()
                .Where(en =>
                    battleContext.GetRelationship(caster.BattleSide, en.BattleSide) == relationship)
                .Where(ent =>
                    !ent.StatusHolder.HasStatus(BattleStatus.Invisible) && ent.EntityType == EntityType.Character);
            return enemies
                .OrderByDescending(e => entitiesBank.GetBasedCharacter(e).CharacterData.Reward)
                .ToArray();
        }
    }
}