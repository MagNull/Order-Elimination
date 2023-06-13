using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EntityMovedTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public bool IgnoreZeroDistanceMoves { get; private set; }

        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
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
