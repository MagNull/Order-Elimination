namespace OrderElimination.AbilitySystem
{
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
}
