namespace OrderElimination.AbilitySystem
{
    public interface IActionPerformResult
    {
        public IBattleAction ModifiedAction { get; }
        public ActionContext ActionContext { get; }
        public bool IsSuccessful { get; }
        //TODO fail reason
    }
}
