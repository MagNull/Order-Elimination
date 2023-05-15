﻿using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public readonly struct TriggerActivationContext
    {
        public readonly ITriggerSetupInfo SetupInfo;
        public readonly IBattleContext BattleContext;
        public readonly AbilitySystemActor TrackingEntity;

        public TriggerActivationContext(
            ITriggerSetupInfo setupInfo, 
            IBattleContext battleContext, 
            AbilitySystemActor trackingEntity = null)
        {
            SetupInfo = setupInfo;
            BattleContext = battleContext;
            TrackingEntity = trackingEntity;
        }
    }

    public interface ITriggerFiredInfo
    {

    }

    public class BattleTrigger//IDisposable
    {
        public bool IsActive { get; private set; }

        public event Action<ITriggerFiredInfo> Triggered;
        public event Action<BattleTrigger> Deactivated;

        private ITriggerSetupInfo _runningSetup;
        private TriggerActivationContext? _operatingContext;

        public void Activate(TriggerActivationContext context)
        {
            if (IsActive) return;//false
            var setup = context.SetupInfo;
            _runningSetup = setup ?? throw new ArgumentNullException(nameof(setup));
            _operatingContext = context;
            _runningSetup.SubscribeTrigger(this, context);
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;//false
            _runningSetup = null;
            _operatingContext = null;
            IsActive = false;
            Deactivated?.Invoke(this);
        }

        [Obsolete("Call " + nameof(ForceTrigger) + "() activation only within " + nameof(ITriggerSetupInfo))]
        internal void ForceTrigger(ITriggerFiredInfo triggerFiredInfo)
        {
            if (!IsActive)
                return;//false
            Triggered?.Invoke(triggerFiredInfo);
        }
    }

    public interface ITriggerSetupInfo
    {
        public void SubscribeTrigger(BattleTrigger battleTrigger, TriggerActivationContext context);
    }
}