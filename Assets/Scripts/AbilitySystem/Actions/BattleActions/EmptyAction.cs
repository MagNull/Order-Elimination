using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EmptyAction : BattleAction<EmptyAction>
    {
        [ShowInInspector, OdinSerialize]
        public ActionRequires ExecutesOn { get; set; } = ActionRequires.Cell;

        [ShowInInspector, OdinSerialize]
        public bool IsSuccessful { get; set; } = true;

        public override ActionRequires ActionRequires => ExecutesOn;

        public override IBattleAction Clone()
        {
            var clone = new EmptyAction();
            clone.ExecutesOn = ExecutesOn;
            clone.IsSuccessful = IsSuccessful;
            return clone;
        }

        protected async override UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            return new SimplePerformResult(this, useContext, IsSuccessful);
        }
    }
}
