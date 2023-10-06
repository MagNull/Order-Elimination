using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EntityMovedTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public bool IgnoreZeroDistanceMoves { get; private set; }

        public IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor activator, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext, activator);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                trackingEntity.MovedFromTo += OnEntityMoved;
            }

            void OnEntityMoved(Vector2Int from, Vector2Int to)
            {
                if (IgnoreZeroDistanceMoves && from == to)
                    return;
                instance.FireTrigger(new EmptyTriggerFireInfo(instance));
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                trackingEntity.MovedFromTo -= OnEntityMoved;
            }
        }

        
    }
}
