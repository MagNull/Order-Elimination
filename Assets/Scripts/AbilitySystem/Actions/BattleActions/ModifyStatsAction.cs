using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class ModifyStatsAction : BattleAction<ModifyStatsAction>, IUndoableBattleAction
    {
        private static List<Func<float, float>> _processorsByPerformId = new();
        private static List<IUndoableActionPerformResult> _resultsByPerformId = new();
        private static HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        public BattleStat TargetBattleStat { get; set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter ValueModifier { get; set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public void ClearUndoCache()
        {
            _processorsByPerformId.Clear();
            _resultsByPerformId.Clear();
            _undoneOperations.Clear();
        }

        public override IBattleAction Clone()
        {
            var clone = new ModifyStatsAction();
            clone.TargetBattleStat = TargetBattleStat;
            clone.ValueModifier = ValueModifier;
            return clone;
        }

        public bool IsUndone(int performId) => _undoneOperations.Contains(performId);

        public bool Undo(int performId) => UndoStatic(performId);

        public static bool UndoStatic(int performId)
        {
            if (_undoneOperations.Contains(performId))
                Logging.LogException( ActionUndoFailedException.AlreadyUndoneException);
            var performResult = _resultsByPerformId[performId];
            var performProcessor = _processorsByPerformId[performId];
            var action = (ModifyStatsAction)performResult.ModifiedAction;
            var targetStat = action.TargetBattleStat;
            if (performResult.IsSuccessful)
            {
                var stats = performResult.ActionContext.ActionTarget.BattleStats;
                var parameter = stats[targetStat];
                if (parameter.RemoveProcessor(performProcessor))
                {
                    _undoneOperations.Add(performId);
                    return true;
                }
                return false;
            }
            return false;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var calculationContext = ValueCalculationContext.Full(useContext);
            var statParameter = useContext.ActionTarget.BattleStats[TargetBattleStat];
            statParameter.AddProcessor(ProcessValue);
            var performId = _processorsByPerformId.Count;
            var result = new SimpleUndoablePerformResult(this, useContext, true, performId);
            _processorsByPerformId.Add(ProcessValue);
            _resultsByPerformId.Add(result);
            return result;

            float ProcessValue(float v) => ValueModifier.GetValue(calculationContext);
        }
    }
}
