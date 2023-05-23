using OrderElimination.AbilitySystem.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class BattleEffect // IDisposable
    {
        public IEffectData EffectData { get; }
        public IBattleContext BattleContext { get; }
        public bool IsActive { get; private set; }
        public AbilitySystemActor EffectApplier { get; private set; }
        public AbilitySystemActor EffectHolder { get; private set; }
        public event Action<BattleEffect> Deactivated;

        //Temporary effect
        public int? LeftDuration { get; private set; }
        public event Action<BattleEffect> DurationEnded;


        public BattleEffect(
            IEffectData effectData,
            IBattleContext battleContext)
        {
            EffectData = effectData;
            BattleContext = battleContext;
        }

        public bool Apply(AbilitySystemActor onEffectHolder, AbilitySystemActor byEffectApplier)
        {
            if (!EffectData.CanBeAppliedOn(onEffectHolder))
                return false;
            EffectHolder = onEffectHolder;
            EffectApplier = byEffectApplier;
            if (EffectData.StackingPolicy == EffectStackingPolicy.OverrideOld
                && EffectHolder.HasEffect(EffectData))
            {
                var effectsToRemove = EffectHolder.Effects.Where(e => e.EffectData == EffectData).ToArray();
                foreach (var oldEffect in effectsToRemove)
                {
                    oldEffect.Deactivate();
                }
            }
            return true;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Attempt to activate effect again.");
            if (EffectHolder == null)
                throw new InvalidOperationException("Effect hasn't been applied on entity yet.");
            
            if (EffectData.View.AnimationOnActivation != null)
            {
                var cellGroups = CellGroupsContainer.Empty;
                var animationContext = new AnimationPlayContext(
                BattleContext.AnimationSceneContext, cellGroups, EffectApplier, EffectHolder);
                EffectData.View.AnimationOnActivation.Play(animationContext);
            }

            EffectData.OnActivation(this);
            IsActive = true;
            EffectData.InstructionOnActivation?.Execute(this);
            if (EffectData.TemporaryEffectFunctionaity != null)
            {
                LeftDuration = EffectData.TemporaryEffectFunctionaity.ApplyingDuration;
                DurationEnded += EffectData.TemporaryEffectFunctionaity.OnTimeOut;
                var activationSide = BattleContext.ActiveSide;
                BattleContext.NewTurnStarted -= OnNewTurn;
                BattleContext.NewTurnStarted += OnNewTurn;

                void OnNewTurn(IBattleContext context)
                {
                    if (!IsActive)
                    {
                        BattleContext.NewTurnStarted -= OnNewTurn;
                        return;
                    }
                    if (context.ActiveSide != activationSide) return;
                    LeftDuration--;
                    if (LeftDuration > 0) return;
                    BattleContext.NewTurnStarted -= OnNewTurn;
                    EffectData.TemporaryEffectFunctionaity.OnTimeOut(this);
                    Deactivate();
                }
            }//Very similar to Timer Trigger. Move to Triggers?
            if (EffectData.TriggerInstructions != null)
            {
                foreach (var triggerInstruction in EffectData.TriggerInstructions)
                {
                    var trigger = triggerInstruction.Key.GetTrigger(BattleContext, EffectHolder, EffectApplier);
                    trigger.Triggered += OnTriggered;
                    Deactivated += DeactivateTriggersOnEffectDeactivation;
                    trigger.Activate();

                    void OnTriggered(ITriggerFireInfo firedInfo)
                    {
                        triggerInstruction.Value.Execute(this);
                    }

                    void DeactivateTriggersOnEffectDeactivation(BattleEffect effect)
                    {
                        trigger.Triggered -= OnTriggered;
                        Deactivated -= DeactivateTriggersOnEffectDeactivation;
                        trigger.Deactivate();
                    }
                }
            }
            foreach (var triggerAcceptor in EffectData.RemoveTriggers)
            {
                var trigger = triggerAcceptor.GetTrigger(BattleContext, EffectHolder, EffectApplier);
                trigger.Triggered += OnTriggered;
                Deactivated += OnEffectDeactivation;
                trigger.Activate();

                void OnTriggered(ITriggerFireInfo fireInfo)
                {
                    Deactivate();
                }

                void OnEffectDeactivation(BattleEffect effect)
                {
                    trigger.Triggered -= OnTriggered;
                    Deactivated -= OnEffectDeactivation;
                    trigger.Deactivate();
                }
            }
        }

        public bool TryDeactivate()
        {
            if (!IsActive) return false;
            Deactivate();
            return true;
        }

        private void Deactivate()
        {
            if (!IsActive) throw new InvalidOperationException();
            if (EffectData.View.AnimationOnDeactivation != null)
            {
                var cellGroups = CellGroupsContainer.Empty;
                var animationContext = new AnimationPlayContext(
                BattleContext.AnimationSceneContext, cellGroups, EffectApplier, EffectHolder);
                EffectData.View.AnimationOnDeactivation.Play(animationContext);
            }
            EffectData.InstructionOnDeactivation?.Execute(this);
            EffectData.OnDeactivation(this);
            IsActive = false;
            Deactivated?.Invoke(this);
        }

        //TemporaryFunctionality
        //ProcessingFunctionality
        //PeriodicFunctionality
    }
}
