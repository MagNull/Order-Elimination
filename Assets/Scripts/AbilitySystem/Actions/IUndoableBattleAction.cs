namespace OrderElimination.AbilitySystem
{
    //Replace with Temporary action, which undoes itself when Triggers fired. (Track actionTarget entity)
    public interface IUndoableBattleAction
    {
        public bool Undo(AbilitySystemActor undoTarget);
    }
}
