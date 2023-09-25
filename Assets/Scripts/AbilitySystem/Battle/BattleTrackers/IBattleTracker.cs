using System;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleTracker// : IDisposable
    {
        public bool IsConditionMet { get; }

        public event Action<IBattleTracker> ConditionMet;
        public event Action<IBattleTracker> ConditionLost;

        public void StartTracking(IBattleContext battleContext);
        public void StopTracking();
    }
}
