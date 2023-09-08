using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectAction : BattleAction<ApplyEffectAction>, 
        IUndoableBattleAction
    {
        private static List<BattleEffect> _appliedEffects = new();
        private static List<IEffectHolder> _performTargets = new();
        private static HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        public IEffectData Effect { get; set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter ApplyChance { get; set; } = new ConstValueGetter(1);

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public override IBattleAction Clone()
        {
            var clone = new ApplyEffectAction();
            clone.Effect = Effect;
            clone.ApplyChance = ApplyChance;
            return clone;
        }

        public bool IsUndone(int performId) => _undoneOperations.Contains(performId);

        public bool Undo(int performId)
        {
            if (_performTargets[performId].RemoveEffect(_appliedEffects[performId]))
            {
                _undoneOperations.Add(performId);
                return true;
            }
            return false;
        }

        public void ClearUndoCache()
        {
            _appliedEffects.Clear();
            _performTargets.Clear();
            _undoneOperations.Clear();
        }

        protected async override UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var isSuccessfull = false;
            var calculationContext = ValueCalculationContext.FromActionContext(useContext);
            var probability = ApplyChance.GetValue(calculationContext);
            BattleEffect appliedEffect = null;
            if (RandomExtensions.RollChance(probability))
            {
                isSuccessfull = useContext.ActionTarget.ApplyEffect(Effect, useContext.ActionMaker, out appliedEffect);
            }
            _appliedEffects.Add(appliedEffect);
            _performTargets.Add(useContext.ActionTarget);
            return new SimpleUndoablePerformResult(this, useContext, isSuccessfull, _appliedEffects.Count - 1);
        }
    }
}
