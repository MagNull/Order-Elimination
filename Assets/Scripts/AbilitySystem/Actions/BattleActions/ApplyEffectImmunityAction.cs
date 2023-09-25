using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectImmunityAction : BattleAction<ApplyEffectImmunityAction>,
        IUndoableBattleAction,
        ICallbackingBattleAction// ? when immunity blocked effect
    {
        private readonly static List<IEffectHolder> _targets = new();
        private readonly static List<IEffectData[]> _effectImmunities = new();
        private readonly static HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        private List<IEffectData> EffectImmunities { get; set; } = new();

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public string CallbackDescription => "Callback happens when immunity blocked an attempt to apply effect.";

        protected event Action<IBattleActionCallback> Callbacks;

        public void ClearUndoCache()
        {
            _targets.Clear();
            _effectImmunities.Clear();
            _undoneOperations.Clear();
        }

        public override IBattleAction Clone()
        {
            var clone = new ApplyEffectImmunityAction();
            clone.EffectImmunities = EffectImmunities.ToList();
            clone.Callbacks = Callbacks;//dont copy callbacks?
            return clone;
        }

        public bool IsUndone(int performId) => _undoneOperations.Contains(performId);

        public async UniTask<IActionPerformResult> ModifiedPerformWithCallbacks(
            ActionContext useContext, 
            Action<IBattleActionCallback> onCallback, 
            bool actionMakerProcessing = true, 
            bool targetProcessing = true)
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

        public bool Undo(int performId)
        {
            if (IsUndone(performId))
            {
                Logging.LogException(ActionUndoFailedException.AlreadyUndoneException);
                return false;
            }
            var target = _targets[performId];
            target.EffectBlockedByImmunity -= OnEffectBlocked;
            var immunities = _effectImmunities[performId];
            var success = true;
            foreach (var effect in immunities)
            {
                if (target.RemoveEffectImmunity(effect))
                    success = false;
            }
            return success;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var performId = _targets.Count;
            var target = useContext.ActionTarget;
            target.EffectBlockedByImmunity += OnEffectBlocked;
            foreach (var effect in EffectImmunities)
            {
                target.AddEffectImmunity(effect);
            }
            _targets.Add(target);
            _effectImmunities.Add(EffectImmunities.ToArray());
            return new SimpleUndoablePerformResult(this, useContext, true, performId);
        }

        private void OnEffectBlocked(IEffectData effect)
        {
            if (EffectImmunities.Contains(effect))
                Callbacks?.Invoke(new ApplyEffectImmunityActionCallback(effect));
        }
    }
}
