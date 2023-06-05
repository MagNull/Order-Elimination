using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;

namespace AI.Actions
{
    public class MoveToNearestShelter : IBehaviorTreeTask
    {
        public bool Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var structures = battleContext.EntitiesBank.GetEntities(BattleSide.NoSide);
            var movementAbility = caster.ActiveAbilities
                .First(a => a.AbilityData.View.Name == "Перемещение");
            var targeting = (SingleTargetTargetingSystem)movementAbility.AbilityData.TargetingSystem;
            movementAbility.InitiateCast(battleContext, caster);
            
            foreach (var structure in structures)
            {
                Debug.Log("Start " + structure.Position);
                if (!targeting.AvailableCells.Contains(structure.Position))
                    continue;
                targeting.ConfirmationUnlocked += _ =>
                {
                    Debug.Log("Confirm");
                    targeting.ConfirmTargeting();
                };
                Debug.Log("Select " + structure.Position);
                
                targeting.Select(structure.Position);
                return true;
            }
            return false;
        }
    }
}