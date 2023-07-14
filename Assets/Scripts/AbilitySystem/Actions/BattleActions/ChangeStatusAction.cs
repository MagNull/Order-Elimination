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
    public class ChangeStatusAction : BattleAction<ChangeStatusAction>, IUndoableBattleAction
    {
        private readonly struct ChangeStatusOperation
        {
            public readonly BattleStatus AffectedStatus;
            public readonly StatusOperation StatusOperation;
            public readonly bool IsSuccessful;
            public readonly EntityStatusHolder Target;

            public ChangeStatusOperation(
                BattleStatus affectedStatus, 
                StatusOperation statusOperation, 
                bool isSuccessful,
                EntityStatusHolder target)
            {
                AffectedStatus = affectedStatus;
                StatusOperation = statusOperation;
                IsSuccessful = isSuccessful;
                Target = target;
            }
        }

        private readonly static List<ChangeStatusOperation> _operations = new();
        private readonly static HashSet<int> _undoneOperations = new();

        public enum StatusOperation
        {
            Increase,
            Decrease,
            //Clear
        }

        [ShowInInspector, OdinSerialize]
        public BattleStatus Status { get; private set; }

        [ShowInInspector, OdinSerialize]
        public StatusOperation Operation { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public override IBattleAction Clone()
        {
            var clone = new ChangeStatusAction();
            clone.Status = Status;
            clone.Operation = Operation;
            return clone;
        }

        public bool IsUndone(int performId) => _undoneOperations.Contains(performId);

        public bool Undo(int performId)
        {
            if (IsUndone(performId))
                Logging.LogException( ActionUndoFailedException.AlreadyUndoneException);
            var operation = _operations[performId];
            if (!operation.IsSuccessful) return false;
            switch (operation.StatusOperation)
            {
                case StatusOperation.Increase:
                    if (!operation.Target.DecreaseStatus(operation.AffectedStatus))
                        Logging.LogException( new ActionUndoFailedException($"Failed to undo {nameof(ChangeStatusAction)} action"));
                    break;
                case StatusOperation.Decrease:
                    operation.Target.IncreaseStatus(operation.AffectedStatus);
                    break;
                default:
                    throw new NotImplementedException();
            }
            _undoneOperations.Add(performId);
            return true;
        }

        public void ClearUndoCache()
        {
            _operations.Clear();
            _undoneOperations.Clear();
        }

        protected async override UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var target = useContext.ActionTarget.StatusHolder;
            bool success;
            switch (Operation)
            {
                case StatusOperation.Increase:
                    target.IncreaseStatus(Status);
                    success = true;
                    break;
                case StatusOperation.Decrease:
                    success = target.DecreaseStatus(Status);
                    break;
                default:
                    throw new NotImplementedException();
            }
            var performId = _operations.Count;
            _operations.Add(new ChangeStatusOperation(Status, Operation, success, target));
            return new SimpleUndoablePerformResult(this, useContext, success, performId);
        }
    }
}
