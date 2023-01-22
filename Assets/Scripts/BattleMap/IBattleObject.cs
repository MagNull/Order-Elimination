using CharacterAbility.BuffEffects;

//TODO: Refactor interfaces IBattleObject
public interface IBattleObject : ITickTarget,
    IBuffTarget, IMovable
{
    public BattleObjectSide Side { get; }
    public IBattleObjectView View { get; }

    public int GetAccuracyFrom(IBattleObject attacker);
}