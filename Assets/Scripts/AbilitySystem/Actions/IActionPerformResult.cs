using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IActionPerformResult
    {
        public IBattleAction ModifiedAction { get; }
        public ActionContext ActionContext { get; }
        public bool IsSuccessful { get; }
        //TODO fail reason
    }

    public interface IUndoableActionPerformResult : IActionPerformResult
    {
        IBattleAction IActionPerformResult.ModifiedAction => ModifiedAction;
        public new IUndoableBattleAction ModifiedAction { get; }
        public int PerformId { get; }
    }

    public sealed class SimplePerformResult : IActionPerformResult
    {
        public IBattleAction ModifiedAction { get; }
        public ActionContext ActionContext { get; }
        public bool IsSuccessful { get; }

        public SimplePerformResult(IBattleAction modifiedAction, ActionContext context, bool isSuccessful)
        {
            ModifiedAction = modifiedAction;
            ActionContext = context;
            IsSuccessful = isSuccessful;
        }
    }

    public sealed class SimpleUndoablePerformResult : IUndoableActionPerformResult
    {
        public IUndoableBattleAction ModifiedAction { get; }
        public ActionContext ActionContext { get; }
        public bool IsSuccessful { get; }
        public int PerformId { get; }

        public SimpleUndoablePerformResult(
            IUndoableBattleAction modifiedAction, ActionContext context, bool isSuccessful, int performId)
        {
            ModifiedAction = modifiedAction;
            ActionContext = context;
            IsSuccessful = isSuccessful;
            PerformId = performId;
        }
    }
}
