using System;
using CharacterAbility;
using OrderElimination;

public interface ITickEffect : IEquatable<ITickEffect>
{
    public bool IsUnique { get; }
    public void Tick(ITickTarget target);

    public ITickEffectView GetEffectView();

    public void RemoveTickEffect(ITickTarget target);
}
