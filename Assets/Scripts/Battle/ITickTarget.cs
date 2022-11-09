public interface ITickTarget
{
    void AddTickEffect(ITickEffect effect);
    void RemoveTickEffect(ITickEffect effect);
    void ClearTickEffects();
}