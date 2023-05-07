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
    public class ApplyEffectAction : BattleAction<ApplyEffectAction>
    {
        [ShowInInspector, OdinSerialize]
        public IEffectData Effect { get; set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter ApplyChance { get; set; }

        public override ActionRequires ActionExecutes => ActionRequires.Entity;

        protected async override UniTask<bool> Perform(ActionContext useContext)
        {
            if (RandomExtensions.RollChance(ApplyChance.GetValue(useContext)))
            {
                var isSuccessfull = useContext.ActionTarget.ApplyEffect(Effect, useContext.ActionMaker);
                var targetView = useContext.BattleContext.EntitiesBank.GetViewByEntity(useContext.ActionTarget);
                if (isSuccessfull)
                    Debug.Log($"Effect {Effect.View.Name} has been applied on {targetView.Name}." % Colorize.Orange);
                return isSuccessfull;
            }
            return false;
        }
    }
}
