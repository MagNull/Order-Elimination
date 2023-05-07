using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public struct TriggerActivationInfo
    {
        public readonly IBattleContext BattleContext;
        public readonly AbilitySystemActor TrackingEntity;

        public TriggerActivationInfo(IBattleContext battleContext, AbilitySystemActor trackingEntity = null)
        {
            BattleContext = battleContext;
            TrackingEntity = trackingEntity;
        }
    }

    public interface ITriggerFiredInfo
    {

    }

    public class TimerTriggerFiredInfo : ITriggerFiredInfo
    {

    }

    public interface IBattleTrigger//IDisposable
    {
        public bool IsActive { get; }

        public event Action<ITriggerFiredInfo> Triggered;

        public void Activate(TriggerActivationInfo battleContext);
        public void Deactivate();
    }

    public class RoundTimerTrigger : IBattleTrigger
    {
        private int _startRound;
        private int _interval;
        private IBattleContext _battleContext;

        public int Interval
        {
            get => _interval;
            set
            {
                if (IsActive) throw new InvalidOperationException("Changing interval on activated timer is not allowed.");
                if (value <= 1) value = 1;
                _interval = value;
            }
        }
        public bool FireOnStart { get; set; }
        public bool StartFromNextRound { get; set; }
        public bool IsActive { get; private set; }

        public event Action<ITriggerFiredInfo> Triggered;

        public void Activate(TriggerActivationInfo activationInfo)
        {
            if (IsActive)
                return;
            if (_battleContext != null)
                _battleContext.NewRoundStarted -= OnNewRoundStarted;
            _battleContext = activationInfo.BattleContext;
            _battleContext.NewRoundStarted += OnNewRoundStarted;
            IsActive = true;
            _startRound = _battleContext.CurrentRound;
            if (StartFromNextRound)
                _startRound++;
            if (FireOnStart && !StartFromNextRound)
            {
                OnNewRoundStarted(_battleContext);
            }
        }

        private void OnNewRoundStarted(IBattleContext context)
        {
            if (!IsActive || (context.CurrentRound - _startRound) % Interval != 0)
                return;
            var passedIntervals = (context.CurrentRound - _startRound) / Interval;
            if (passedIntervals == 0 && !FireOnStart)
                return;
            var triggerInfo = new TimerTriggerFiredInfo();//Send passedIntervals
            Debug.Log("Tick");
            Triggered?.Invoke(triggerInfo);
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;
            IsActive = false;
            _battleContext.NewRoundStarted -= OnNewRoundStarted;
        }
    }
}
