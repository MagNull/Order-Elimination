namespace OrderElimination.AbilitySystem
{
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
