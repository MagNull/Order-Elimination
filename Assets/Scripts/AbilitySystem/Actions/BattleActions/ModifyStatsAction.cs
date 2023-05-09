﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace OrderElimination.AbilitySystem
{
    public class ModifyStatsAction : BattleAction<ModifyStatsAction>, IUndoableBattleAction
    {
        private static List<Func<float, float>> _processorsByPerformId = new();
        private static List<IUndoableActionPerformResult> _resultsByPerformId = new();

        [ShowInInspector, OdinSerialize]
        public BattleStat TargetBattleStat { get; set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter ValueModifier { get; set; }

        public override ActionRequires ActionRequires => ActionRequires.Entity;

        public void ClearUndoCache()
        {
            _processorsByPerformId.Clear();
            _resultsByPerformId.Clear();
        }

        public override IBattleAction Clone()
        {
            var clone = new ModifyStatsAction();
            clone.TargetBattleStat = TargetBattleStat;
            clone.ValueModifier = ValueModifier;
            return clone;
        }

        public bool Undo(int performId) => UndoStatic(performId);

        public static bool UndoStatic(int performId)
        {
            var performResult = _resultsByPerformId[performId];
            var performProcessor = _processorsByPerformId[performId];
            var action = (ModifyStatsAction)performResult.ModifiedAction;
            var targetStat = action.TargetBattleStat;
            if (performResult.IsSuccessful)
            {
                var stats = performResult.ActionContext.ActionTarget.BattleStats;
                var parameter = stats.GetParameter(targetStat);
                return parameter.RemoveProcessor(performProcessor);
            }
            return false;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var statParameter = useContext.ActionTarget.BattleStats.GetParameter(TargetBattleStat);
            statParameter.AddProcessor(ProcessValue);
            var performId = _processorsByPerformId.Count - 1;
            _processorsByPerformId.Add(ProcessValue);
            return new SimpleUndoablePerformResult(this, useContext, true, performId);

            float ProcessValue(float v) => ValueModifier.GetValue(useContext);
        }
    }
}
