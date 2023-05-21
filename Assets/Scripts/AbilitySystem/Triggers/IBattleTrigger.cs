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

        public event Action<ITriggerFireInfo> Triggered;

        public bool Activate();
        public bool Deactivate(); //Dispose
    }

    public interface ITriggerFireInfo
    {

    }

    public class EmptyTriggerFireInfo : ITriggerFireInfo
    {

    }
}
