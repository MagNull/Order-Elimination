using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleTrigger
    {
        public bool IsActive { get; }

        //TODO: Replace with Func<ITriggerFireInfo, UniTask>. Await on Invoke()
        public event Action<ITriggerFireInfo> Triggered;
        public event Action<IBattleTrigger> Deactivated;
        //public event Action<IBattleTrigger> AllTriggerHandlersExecuted;

        public bool Activate();
        public bool Deactivate(); //Dispose
    }
}
