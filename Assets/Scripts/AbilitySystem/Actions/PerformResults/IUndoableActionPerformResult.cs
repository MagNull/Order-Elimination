namespace OrderElimination.AbilitySystem
{
    public interface IUndoableActionPerformResult : IActionPerformResult
    {
        IBattleAction IActionPerformResult.ModifiedAction => ModifiedAction;
        public new IUndoableBattleAction ModifiedAction { get; }
        public int PerformId { get; }
    }
}
