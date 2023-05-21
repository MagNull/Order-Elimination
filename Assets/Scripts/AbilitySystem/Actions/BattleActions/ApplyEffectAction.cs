using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem;
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
    public class ApplyEffectAction : BattleAction<ApplyEffectAction>//, IUndoableBattleAction
    {
        [ShowInInspector, OdinSerialize]
        public IEffectData Effect { get; set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter ApplyChance { get; set; }

        public override ActionRequires ActionRequires => ActionRequires.Entity;

        public override IBattleAction Clone()
        {
            var clone = new ApplyEffectAction();
            clone.Effect = Effect;
            clone.ApplyChance = ApplyChance;
            return clone;
        }

        protected async override UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var isSuccessfull = false;
            var probability = ApplyChance.GetValue(useContext);
            if (RandomExtensions.RollChance(probability))
            {
                isSuccessfull = useContext.ActionTarget.ApplyEffect(Effect, useContext.ActionMaker);
            }
            return new SimplePerformResult(this, useContext, isSuccessfull);
        }
    }
}
