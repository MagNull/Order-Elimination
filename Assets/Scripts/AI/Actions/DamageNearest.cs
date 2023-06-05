using System.Linq;
using AI.Utils;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;

namespace AI.Actions
{
    public class DamageNearest : IBehaviorTreeTask
    {
        //TODO: Check distance to target
        public bool Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var target = GetNearestEnemy(battleContext.EntitiesBank, caster);
            var distanceToTarget = battleContext.BattleMap.GetGameDistanceBetween(caster.Position, target.Position);
            
            var damageAbilities = caster.ActiveAbilities
                .Where(ability => ability.IsCastAvailable(battleContext, caster))
                .Select(ability => (ability, new AbilityImpact(ability.AbilityData, battleContext, caster, target)))
                .Where(impact => impact.Item2.Damage > 0);
            
            if(!damageAbilities.Any())
                return false;
            
            var maxDamageAbility =
                damageAbilities.OrderByDescending(impact => impact.Item2.Damage).First();

            switch (maxDamageAbility.ability.AbilityData.TargetingSystem)
            {
                case SingleTargetTargetingSystem singleTargeting:
                {
                    singleTargeting.ConfirmationUnlocked += _ =>
                    {
                        singleTargeting.ConfirmTargeting();
                    };

                    singleTargeting.Select(target.Position);
                    break;
                }
            }

            return true;
        }

        private AbilitySystemActor GetNearestEnemy(IReadOnlyEntitiesBank entitiesBank, AbilitySystemActor caster)
        {
            var enemies = entitiesBank.GetEntities(BattleSide.Enemies);
            var nearestEnemy = enemies.First();
            var nearestDistance = float.MaxValue;
            foreach (var enemy in enemies)
            {
                var distance = (enemy.Position - caster.Position).magnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }
    }
}