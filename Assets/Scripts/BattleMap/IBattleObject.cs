using CharacterAbility.BuffEffects;

//TODO: Refactor interfaces IBattleObject
public interface IBattleObject : ITickTarget,
    IBuffTarget, IMovable
{
    public BattleObjectType Type { get; }

    public bool IsAlive { get; }
    public IBattleObjectView View { get; }

    public int GetAccuracyFrom(IBattleObject attacker);
}