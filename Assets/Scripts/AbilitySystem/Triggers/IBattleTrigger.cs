using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem.Triggers
{
    public interface ITriggerFiredInfo
    {

    }

    public interface IBattleTrigger
    {
        public event Action<ITriggerFiredInfo> Triggered;
    }

    //public interface IContextTrigger : IBattleTrigger
    //{
    //    public void Activate(IBattleContext battleContext);
    //}

    //public interface IEntityTrackingTrigger : IBattleTrigger
    //{
    //    public void Activate(IBattleContext battleContext, AbilitySystemActor trackingEntity);
    //}
}
