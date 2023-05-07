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

        public override ActionRequires ActionExecutes => ExecutesOn;

        protected async override UniTask<bool> Perform(ActionContext useContext)
        {
            return IsSuccessful;
        }
    }
}
