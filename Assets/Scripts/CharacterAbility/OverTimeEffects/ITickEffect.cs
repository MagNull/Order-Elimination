using CharacterAbility;
using OrderElimination;

public interface ITickEffect
{
    public void Tick(ITickTarget target);

    public ITickEffectView GetEffectView();

    public void RemoveTickEffect(ITickTarget target);
}
