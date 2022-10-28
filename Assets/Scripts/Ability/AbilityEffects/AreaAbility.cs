namespace Ability.AbilityEffects
{
    public class AreaAbility : IAbility
    {
        private readonly int _radius;
        private readonly IAbility _areaAbility;

        public AreaAbility(IAbility areaAbility, int radius)
        {
            _radius = radius;
            _areaAbility = areaAbility;
        }

        public void Use(IBattleObject caster, IBattleObject target, BattleMapView battleMapView)
        {
            var targets = battleMapView.Map.GetObjectsInRadius(target, _radius);
            foreach (var battleObject in targets)
            {
                _areaAbility.Use(caster, battleObject, battleMapView);
            }
        }
    }
}