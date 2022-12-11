public interface ITickTarget : IDamageable, IHealable
{
    void AddTickEffect(ITickEffect effect);
    void RemoveTickEffect(ITickEffect effect);
    void ClearTickEffects();
}