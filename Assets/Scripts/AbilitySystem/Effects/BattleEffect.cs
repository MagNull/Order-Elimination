using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private RoundTimerTrigger _durationTimer;
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
                _durationTimer = new RoundTimerTrigger();
                _durationTimer.Interval = LeftDuration.Value;
                _durationTimer.StartFromNextRound = false;
                _durationTimer.FireOnStart = true;
                _durationTimer.Triggered += OnTimeOut;
                var triggerInfo = new TriggerActivationInfo(BattleContext);
                _durationTimer.Activate(triggerInfo);
            }
            return true;

            void OnTimeOut(ITriggerFiredInfo triggerInfo)
            {
                _durationTimer.Deactivate();
                _durationTimer = null;
                EffectData.TemporaryEffectFunctionaity.OnTimeOut(this);
                Deactivate();
            }
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
            Deactivated?.Invoke(this);
        }

        //TemporaryFunctionality
        //ProcessingFunctionality
        //PeriodicFunctionality
    }
}
