using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerAwaitable
    {
        public IReadOnlyList<IExecutionTrigger> Triggers => _triggers;
        protected List<IExecutionTrigger> _triggers { get; }

        public void AddTrigger(IExecutionTrigger trigger)
        {
            _triggers.Add(trigger);
            trigger.Trigerred += OnTriggerActivated;
        }

        public void RemoveTrigger(IExecutionTrigger trigger)
        {
            if (!_triggers.Contains(trigger))
                return;
            _triggers.Remove(trigger);
            trigger.Trigerred -= OnTriggerActivated;
        }

        protected void OnTriggerActivated(IExecutionTrigger trigger);
    }

    public class TriggerTest : ITriggerAwaitable
    {
        List<IExecutionTrigger> ITriggerAwaitable._triggers => throw new NotImplementedException();

        void ITriggerAwaitable.OnTriggerActivated(IExecutionTrigger trigger)
        {
            throw new NotImplementedException();
        }
    }
}
