using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectAction : BattleAction<ApplyEffectAction>, 
        IUndoableBattleAction,
        ICallbackingBattleAction
    {
        private static List<BattleEffect> _appliedEffects = new();
        private static List<IEffectHolder> _performTargets = new();
        private static HashSet<int> _undoneOperations = new();

        protected event Action<IBattleActionCallback> Callbacks;

        [ShowInInspector, OdinSerialize]
        public IEffectData Effect { get; set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter ApplyChance { get; set; } = new ConstValueGetter(1);

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public string CallbackDescription => "Callback happens when effect is deactivated.";

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
            var calculationContext = ValueCalculationContext.Full(useContext);
            var probability = ApplyChance.GetValue(calculationContext);
            BattleEffect appliedEffect = null;
            if (RandomExtensions.RollChance(probability))
            {
                if (useContext.ActionTarget.ApplyEffect(Effect, useContext.ActionMaker, out appliedEffect))
                {
                    isSuccessfull = true;
                    appliedEffect.Deactivated += OnEffectRemoved;
                }
            }
            _appliedEffects.Add(appliedEffect);
            _performTargets.Add(useContext.ActionTarget);
            return new SimpleUndoablePerformResult(this, useContext, isSuccessfull, _appliedEffects.Count - 1);

            void OnEffectRemoved(BattleEffect effect)
            {
                effect.Deactivated -= OnEffectRemoved;
                Callbacks?.Invoke(new ApplyEffectActionCallback(effect));
            }
        }

        public async UniTask<IActionPerformResult> ModifiedPerformWithCallbacks(ActionContext useContext, Action<IBattleActionCallback> onCallback, bool actionMakerProcessing = true, bool targetProcessing = true)
        {
            if (ActionRequires == ActionRequires.Target)
            {
                if (useContext.ActionTarget == null)
                    throw new ArgumentNullException("Attempt to perform action on null entity.");
                if (useContext.ActionTarget.IsDisposedFromBattle)
                    throw new InvalidOperationException("Attempt to perform action on entity that had been disposed.");
            }
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            modifiedAction.Callbacks += onCallback;
            var performResult = await modifiedAction.Perform(useContext);
            return performResult;
        }
    }
}
