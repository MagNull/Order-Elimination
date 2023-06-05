using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;

namespace AI.Actions
{
    public class MoveToNearestShelter : IBehaviorTreeTask
    {
        public async UniTask<bool> Run(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var structures = battleContext.EntitiesBank.GetEntities(BattleSide.NoSide)
                .Where(actor => actor.Obstacle != null);
            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            var targeting = (SingleTargetTargetingSystem)movementAbility.AbilityData.TargetingSystem;
            movementAbility.InitiateCast(battleContext, caster);

            foreach (var structure in structures)
            {
                if (!structure.Obstacle.IsAllowedToStay(caster) ||
                    targeting.AvailableCells == null ||
                    !targeting.AvailableCells.Contains(structure.Position)
                    || CharacterBehavior.AvoidObject.Contains(battleContext.EntitiesBank.GetBattleStructureData(structure)))
                    continue;
                targeting.ConfirmationUnlocked += _ => { targeting.ConfirmTargeting(); };
                targeting.Select(structure.Position);

                var completed = false;
                movementAbility.AbilityCastCompleted += _ => completed = true;
                await UniTask.WaitUntil(() => completed);

                return true;
            }

            return false;
        }
    }
}