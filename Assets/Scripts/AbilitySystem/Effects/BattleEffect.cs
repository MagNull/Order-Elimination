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
            IBattleContext battleContext,
            AbilitySystemActor effectHolder, 
            AbilitySystemActor effectApplier)
        {
            EffectData = effectData;
            BattleContext = battleContext;
            EffectHolder = effectHolder;
            EffectApplier = effectApplier;
        }

        public bool Activate()
        {
            if (IsActive) return false;
            if (!EffectData.CanBeAppliedOn(EffectHolder))
                return false;
            if (EffectData.StackingPolicy == EffectStackingPolicy.OverrideOld 
                && EffectHolder.HasEffect(EffectData))
            {
                var effectsToRemove = EffectHolder.Effects.Where(e => e.EffectData == EffectData).ToArray();
                foreach (var oldEffect in effectsToRemove)
                {
                    oldEffect.Deactivate();
                }
            }
            //foreach removeTrigger
            //foreach trigger ... assign functionality
            
            EffectData.OnActivation(this);
            IsActive = true;
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
            }
            return true;
        }

        public bool TryDeactivate()
        {
            if (!IsActive) return false;
            if (!EffectData.CanBeForceRemoved) return false;
            Deactivate();
            return true;
        }

        private void Deactivate()
        {
            EffectData.OnDeactivation(this);
            IsActive = false;
            Deactivated?.Invoke(this);
        }

        //TemporaryFunctionality
        //ProcessingFunctionality
        //PeriodicFunctionality
    }
}
