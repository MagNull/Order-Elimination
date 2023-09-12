using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
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

        [DisableIf("@" + nameof(HasRandomSuccessChance))]
        [ShowInInspector, OdinSerialize]
        public bool IsSuccessful { get; set; } = true;

        [ShowInInspector, OdinSerialize]
        public bool HasRandomSuccessChance { get; set; } = false;

        [ShowIf("@" + nameof(HasRandomSuccessChance))]
        [ShowInInspector, OdinSerialize]
        public IContextValueGetter SuccessChance { get; set; } = new ConstValueGetter(0.5f);


        public override ActionRequires ActionRequires => ExecutesOn;

        public override IBattleAction Clone()
        {
            var clone = new EmptyAction();
            clone.ExecutesOn = ExecutesOn;
            clone.HasRandomSuccessChance = HasRandomSuccessChance;
            clone.SuccessChance = SuccessChance != null ? SuccessChance.Clone() : null;
            clone.IsSuccessful = IsSuccessful;
            return clone;
        }

        protected async override UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var success = IsSuccessful; 
            if (HasRandomSuccessChance)
            {
                var chance = SuccessChance.GetValue(ValueCalculationContext.Full(useContext));
                success = RandomExtensions.RollChance(chance);
            }
            return new SimplePerformResult(this, useContext, success);
        }
    }
}
