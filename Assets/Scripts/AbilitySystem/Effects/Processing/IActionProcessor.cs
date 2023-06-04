namespace OrderElimination.AbilitySystem
{
    public interface IActionProcessor
    {
        public TAction ProcessAction<TAction>(TAction originalAction, ActionContext performContext)
            where TAction : BattleAction<TAction>;
    }
}
