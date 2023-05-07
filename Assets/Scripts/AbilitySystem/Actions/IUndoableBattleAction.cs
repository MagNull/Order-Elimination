namespace OrderElimination.AbilitySystem
{
    public interface IUndoableBattleAction
    {
        public bool Undo(AbilitySystemActor undoTarget);
    }
}
