using System.Linq;
using OrderElimination.AbilitySystem;

namespace AI.Actions
{
    public class MoveToNearestShelter : IBehaviorTreeTask
    {
        public bool Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var battleMap = battleContext.BattleMap;
            var structures = battleMap.GetAllEntities(t => t.EntityType == EntityType.Structure);
            var movementAbility = caster.ActiveAbilities
                .First(a => a.AbilityData.View.Name == "Передвижение");
            foreach (var structure in structures)
            {
                if (!movementAbility.AbilityData.Rules.GetAvailableCellPositions(battleContext, caster).Contains(structure.Position))
                    continue;
                movementAbility.InitiateCast(battleContext, caster);
                movementAbility.AbilityData.TargetingSystem.
                    return true;
            }
            return false;
        }
    }
}