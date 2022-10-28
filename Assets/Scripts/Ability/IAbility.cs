namespace Ability
{
    public interface IAbility
    {
        void Use(IBattleObject caster, IBattleObject target, BattleMapView battleMapView);
    }
}