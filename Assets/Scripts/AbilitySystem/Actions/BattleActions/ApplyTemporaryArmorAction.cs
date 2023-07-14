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
    public class ApplyTemporaryArmorAction : BattleAction<ApplyTemporaryArmorAction>, IUndoableBattleAction
    {
        private static readonly List<TemporaryArmor> _appliedTempArmors = new();
        private static readonly List<IUndoableActionPerformResult> _actionResults = new();
        private static readonly HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter TemporaryArmorAmount { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public void ClearUndoCache()
        {
            _undoneOperations.Clear();
            _appliedTempArmors.Clear();
            _actionResults.Clear();
        }

        public override IBattleAction Clone()
        {
            var clone = new ApplyTemporaryArmorAction
            {
                TemporaryArmorAmount = TemporaryArmorAmount.Clone(),
            };
            return clone;
        }

        public bool IsUndone(int performId)
        {
            return _undoneOperations.Contains(performId);
        }

        public bool Undo(int performId)
        {
            if (_undoneOperations.Contains(performId))
                return false;
            var result = _actionResults[performId];
            if (!result.IsSuccessful) 
                return false;
            var armor = _appliedTempArmors[performId];
            result.ActionContext.ActionTarget.BattleStats.RemoveTemporaryArmor(armor);
            _undoneOperations.Add(performId);
            return true;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var temporaryArmor = new TemporaryArmor(TemporaryArmorAmount.GetValue(useContext));
            var currentId = _appliedTempArmors.Count;
            useContext.ActionTarget.BattleStats.AddTemporaryArmor(temporaryArmor);
            var actionResult = new SimpleUndoablePerformResult(this, useContext, true, currentId);
            _appliedTempArmors.Add(temporaryArmor);
            _actionResults.Add(actionResult);
            return actionResult;
        }
    }
}
