using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IExecutionTrigger
    {
        public event Action<IExecutionTrigger> Trigerred;
        //Для отложенного срабатывания
        //public bool WaitForManualUpdate { get; set; }
        //public void UpdateTriggerState();
    }

    public interface IExecutionTrigger<TTrigger> : IExecutionTrigger where TTrigger : IExecutionTrigger<TTrigger>
    {
        public event Action<TTrigger> Triggered;
    }

    public class TargetHitTrigger : IExecutionTrigger<TargetHitTrigger>, IExecutionTrigger
    {
        //public IBattleEntity Target;

        public event Action<TargetHitTrigger> Triggered;
        public event Action<IExecutionTrigger> Trigerred;
    }
}
