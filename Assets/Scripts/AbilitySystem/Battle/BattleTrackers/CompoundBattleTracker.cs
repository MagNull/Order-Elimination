using OrderElimination.AbilitySystem.Conditions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class CompoundBattleTracker : IBattleTracker
    {
        private Dictionary<IBattleTracker, bool> _accomplishedTrackers;
        private IBattleContext _trackingContext;

        [ShowInInspector, OdinSerialize]
        public RequireType CompoundOperand { get; private set; }

        [ShowInInspector, OdinSerialize]
        public IBattleTracker[] Trackers { get; private set; } = new IBattleTracker[0];

        public bool IsConditionMet { get; private set; }

        public event Action<IBattleTracker> ConditionMet;
        public event Action<IBattleTracker> ConditionLost;

        public void StartTracking(IBattleContext battleContext)
        {
            StopTracking();
            IsConditionMet = false;
            _trackingContext = battleContext;
            foreach (var tracker in Trackers)
            {
                tracker.ConditionMet += OnTrackerConditionMet;
                tracker.ConditionLost += OnTrackerConditionLost;
            }
            foreach (var tracker in Trackers)
                tracker.StartTracking(_trackingContext);
        }

        public void StopTracking()
        {
            foreach (var tracker in Trackers)
            {
                tracker.StopTracking();
                tracker.ConditionMet -= OnTrackerConditionMet;
                tracker.ConditionLost -= OnTrackerConditionLost;
            }
            _accomplishedTrackers = Trackers.ToDictionary(e => e, e => false);
        }

        private void OnTrackerConditionMet(IBattleTracker tracker)
        {
            _accomplishedTrackers[tracker] = true;
            CheckTrackers();
        }

        private void OnTrackerConditionLost(IBattleTracker tracker)
        {
            _accomplishedTrackers[tracker] = false;
            CheckTrackers();
        }

        private void CheckTrackers()
        {
            var subconditionsMet = TrackingConditionsMet();
            if (!IsConditionMet && subconditionsMet)//Condition wasnt met before but is now
            {
                IsConditionMet = true;
                ConditionMet?.Invoke(this);
            }
            else if (IsConditionMet && !subconditionsMet)//Condition was met before but no longer
            {
                IsConditionMet = false;
                ConditionLost?.Invoke(this);
            }
        }

        private bool TrackingConditionsMet()
        {
            return CompoundOperand switch
            {
                RequireType.All => _accomplishedTrackers.Values.All(e => e),
                RequireType.Any => _accomplishedTrackers.Values.Any(e => e),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
