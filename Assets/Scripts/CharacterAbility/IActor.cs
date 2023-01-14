using System.Collections.Generic;

public interface IActor : IBattleObject
{

    public IReadOnlyList<ActionType> AvailableActions { get; }

    public bool TrySpendAction(ActionType actionType);

    public bool CanSpendAction(ActionType actionType);

    public void AddAction(ActionType actionType);

    public void ClearActions();
}