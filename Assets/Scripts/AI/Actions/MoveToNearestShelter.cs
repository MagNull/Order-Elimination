using System.Linq;
using AI.EditorGraph;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;

namespace AI.Actions
{
    public class MoveToNearestShelter : BehaviorTreeTask
    {
        [SerializeField]
        private PassiveAbilityBuilder[] _needPassiveEffects;

        protected override async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");

            var structures = context.EntitiesBank.GetEntities(BattleSide.NoSide)
                .Where(actor => actor.Obstacle != null);
            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            if (movementAbility.AbilityData.TargetingSystem 
                is not IRequireSelectionTargetingSystem manualTargeting)
                throw new System.NotSupportedException();
            var targeting = manualTargeting;

            if (!movementAbility.InitiateCast(context, caster))
                return false;

            foreach (var structure in structures)
            {
                if (!CheckStructureAllowability(structure, caster, targeting, context))
                    continue;
                targeting.ConfirmationUnlocked += _ => { targeting.ConfirmTargeting(); };
                targeting.Select(structure.Position);

                var completed = false;
                movementAbility.AbilityExecutionCompleted += _ => completed = true;
                Debug.Log("Start");
                await UniTask.WaitUntil(() => completed);
                Debug.Log("End");

                targeting.CancelTargeting();
                return true;
            }

            targeting.CancelTargeting();

            return false;
        }

        private bool CheckStructureAllowability(AbilitySystemActor structure, AbilitySystemActor caster,
            IRequireSelectionTargetingSystem targeting, IBattleContext context)
        {
            return structure.Obstacle.IsAllowedToStay(caster) &&
                   targeting.PeekAvailableCells(context, caster).Contains(structure.Position) &&
                   !CharacterBehavior.AvoidObject.Contains(context.EntitiesBank.GetBattleStructureData(structure))
                   && _needPassiveEffects.All(ef =>
                       structure.PassiveAbilities.Any(ab => ab.AbilityData.BasedBuilder == ef));
        }
    }
}