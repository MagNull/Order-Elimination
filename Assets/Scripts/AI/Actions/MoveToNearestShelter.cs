﻿using System.Linq;
using AI.Utils;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using UnityEngine;

namespace AI.Actions
{
    public class MoveToNearestShelter : IBehaviorTreeTask
    {
        [SerializeField]
        private PassiveAbilityBuilder[] _needPassiveEffects;

        public async UniTask<bool> Run(Blackboard blackboard)
        {
            var context = blackboard.Get<IBattleContext>("context");
            var caster = blackboard.Get<AbilitySystemActor>("caster");

            var structures = context.EntitiesBank.GetEntities(BattleSide.NoSide)
                .Where(actor => actor.Obstacle != null);
            var movementAbility = AbilityAIPresentation.GetMoveAbility(caster);
            var targeting = (SingleTargetTargetingSystem)movementAbility.AbilityData.TargetingSystem;
            
            movementAbility.InitiateCast(context, caster);

            foreach (var structure in structures)
            {
                if (!CheckStructureAllowability(structure, caster, targeting, context))
                    continue;
                targeting.ConfirmationUnlocked += _ => { targeting.ConfirmTargeting(); };
                targeting.Select(structure.Position);

                var completed = false;
                movementAbility.AbilityExecutionCompleted += _ => completed = true;
                await UniTask.WaitUntil(() => completed);

                targeting.CancelTargeting();
                return true;
            }

            targeting.CancelTargeting();

            return false;
        }

        private bool CheckStructureAllowability(AbilitySystemActor structure, AbilitySystemActor caster,
            SingleTargetTargetingSystem targeting, IBattleContext context)
        {
            return structure.Obstacle.IsAllowedToStay(caster) &&
                   targeting.AvailableCells != null &&
                   targeting.AvailableCells.Contains(structure.Position) &&
                   !CharacterBehavior.AvoidObject.Contains(context.EntitiesBank.GetBattleStructureData(structure))
                   && _needPassiveEffects.All(ef =>
                       structure.PassiveAbilities.Any(ab => ab.AbilityData.BasedBuilder == ef));
        }
    }
}