//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
//using System;
//using UnityEngine;

//namespace OrderElimination.AbilitySystem
//{
//    public class RoundTimerTriggerOld //: ITriggerSetupInfo
//    {
//        private int _startRound;
//        private int _interval;
//        private IBattleContext _battleContext;

//        public int Interval
//        {
//            get => _interval;
//            set
//            {
//                if (IsActive) throw new InvalidOperationException("Changing interval on activated timer is not allowed.");
//                if (value <= 1) value = 1;
//                _interval = value;
//            }
//        }
//        public bool FireOnStart { get; set; }
//        public bool StartFromNextRound { get; set; }
//        public bool IsActive { get; private set; }

//        public event Action<ITriggerFiredInfo> Triggered;

//        public void Activate(TriggerActivationContext activationInfo)
//        {
//            if (IsActive)
//                return;
//            if (_battleContext != null)
//                _battleContext.NewRoundBegan -= OnNewRoundStarted;
//            _battleContext = activationInfo.BattleContext;
//            _battleContext.NewRoundBegan += OnNewRoundStarted;
//            IsActive = true;
//            _startRound = _battleContext.CurrentRound;
//            if (StartFromNextRound)
//                _startRound++;
//            if (FireOnStart && !StartFromNextRound)
//            {
//                OnNewRoundStarted(_battleContext);
//            }
//        }

//        private void OnNewRoundStarted(IBattleContext context)
//        {
//            if (!IsActive || (context.CurrentRound - _startRound) % Interval != 0)
//                return;
//            var passedIntervals = (context.CurrentRound - _startRound) / Interval;
//            if (passedIntervals == 0 && !FireOnStart)
//                return;
//            var triggerInfo = new TimerTriggerFiredInfo();//Send passedIntervals
//            Debug.Log("Tick");
//            Triggered?.Invoke(triggerInfo);
//        }

//        public void Deactivate()
//        {
//            if (!IsActive)
//                return;
//            IsActive = false;
//            _battleContext.NewRoundBegan -= OnNewRoundStarted;
//        }
//    }

//    public struct TimerTriggerFiredInfo : ITriggerFiredInfo
//    {

//    }

//    public class RoundTimerTrigger : ITriggerSetupInfo
//    {
//        [HideInInspector, OdinSerialize]
//        private int _interval = 1;

//        [ShowInInspector]
//        public int Interval
//        {
//            get => _interval;
//            set
//            {
//                if (value < 1) value = 1;
//                _interval = value;
//            }
//        }

//        public void SubscribeTrigger(BattleTrigger battleTrigger, TriggerActivationContext context)
//        {
//            var passedRounds = 0;
//            var activationSide = context.BattleContext.ActiveSide;
//            context.BattleContext.NewTurnStarted += OnNewTurn;
//            battleTrigger.Deactivated += OnDeactivation;

//            void OnNewTurn(IBattleContext battleContext)
//            {
//                if (battleContext.ActiveSide != activationSide)
//                    return;
//                passedRounds++;
//                if (passedRounds == Interval)
//                    battleTrigger.ForceTrigger(new TimerTriggerFiredInfo());
//            }

//            void OnDeactivation(BattleTrigger trigger)
//            {
//                context.BattleContext.NewTurnStarted -= OnNewTurn;
//                trigger.Deactivated -= OnDeactivation;
//            }
//        }
//    }
//}
