using System.Collections.Generic;
using OrderElimination;

public interface IAbilityCaster : IBattleObject
{
    IReadOnlyBattleStats Stats { get; }
    public IReadOnlyList<ActionType> AvailableActions { get; }

    public bool TrySpendAction(ActionType actionType);

    public void AddAction(ActionType actionType);
}