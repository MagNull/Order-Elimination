using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace OrderElimination.AbilitySystem
{
    public class HealAction : BattleAction<HealAction>
    {
        private string _healFormula => HealSize.DisplayedFormula;

        [ShowInInspector, OdinSerialize]
        [PropertyTooltip("@" + nameof(_healFormula)), GUIColor(0.5f, 1, 0.5f)]
        public IContextValueGetter HealSize { get; set; }


        [ShowInInspector, OdinSerialize]
        public LifeStatPriority HealPriority { get; set; } = LifeStatPriority.HealthFirst;


        [ShowInInspector, OdinSerialize]
        public float ArmorMultiplier { get; set; } = 1f;


        [ShowInInspector, OdinSerialize]
        public float HealthMultiplier { get; set; } = 1f;

        public override ActionRequires ActionRequires => ActionRequires.Target;

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var value = HealSize.GetValue(useContext);
            var healer = useContext.ActionMaker;
            var healInfo = new RecoveryInfo(value, ArmorMultiplier, HealthMultiplier, HealPriority, healer);
            useContext.ActionTarget.TakeRecovery(healInfo);
            return new SimplePerformResult(this, useContext, true);
        }

        public override IBattleAction Clone()
        {
            var clone = new HealAction();
            clone.HealSize = HealSize.Clone();
            clone.HealPriority = HealPriority;
            clone.ArmorMultiplier = ArmorMultiplier;
            clone.HealthMultiplier = HealthMultiplier;
            return clone;
        }
    }
}
