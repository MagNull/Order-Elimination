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
    public class EntityDidntMoveTrigger : IEntityTriggerSetup
    {
        [HideInInspector, OdinSerialize]
        private int _roundInterval = 1;

        [ShowInInspector]
        public int RoundInterval
        {
            get => _roundInterval;
            set
            {
                if (value < 1) value = 1;
                _roundInterval = value;
            }
        }

        [ShowInInspector, OdinSerialize]
        public bool IgnoreZeroDistanceMoves { get; private set; }

        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.Activated += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                trigger.Activated -= OnActivation;
                var passedRounds = 0;
                var movedInRound = false;
                var interval = RoundInterval;
                var activationSide = trigger.OperatingContext.ActiveSide;
                trigger.OperatingContext.NewTurnStarted += OnNewTurn;
                trigger.Deactivated += OnDeactivation;
                trackingEntity.MovedFromTo += OnEntityMoved;

                void OnEntityMoved(Vector2Int from, Vector2Int to)
                {
                    if (IgnoreZeroDistanceMoves && from == to)
                        return;
                    movedInRound = true;
                }

                void OnNewTurn(IBattleContext battleContext)
                {
                    if (battleContext.ActiveSide != activationSide)
                        return;
                    if (movedInRound)
                    {
                        passedRounds = 0;
                        movedInRound = false;
                        return;
                    }
                    passedRounds++;
                    if (passedRounds % interval == 0)
                        Trigger();
                    movedInRound = false;
                }

                void Trigger()
                {
                    Debug.Log($"Didn't move for {passedRounds}." % Colorize.Red);
                    trigger.Trigger(new TimerTriggerFireInfo(trigger, passedRounds));
                }

                void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
                {
                    trigger.Deactivated -= OnDeactivation;
                    trigger.OperatingContext.NewTurnStarted -= OnNewTurn;
                    trackingEntity.MovedFromTo -= OnEntityMoved;
                }
            }
        }
    }
}
