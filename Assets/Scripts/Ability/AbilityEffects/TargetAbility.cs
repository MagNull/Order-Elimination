namespace Ability.AbilityEffects
{
    public class TargetAbility : IAbility
    {
        private readonly IAbility _nextAbility;
        private readonly int _distance;
        private readonly bool _selfCast;

        public TargetAbility(IAbility nextAbility, int distance, bool selfCast)
        {
            _nextAbility = nextAbility;
            _distance = distance;
            _selfCast = selfCast;
        }

        public async void Use(IBattleObject caster, IBattleObject target, BattleMapView battleMapView)
        {
            var castPos = battleMapView.Map.GetCoordinate(caster);
            battleMapView.LightCellByDistance(castPos.x, castPos.y, _distance);
            //await UniTask.WaitUntil(() => Input.); TODO: Ожидание клика по клетке
            _nextAbility.Use(caster, _selfCast ? caster : target, battleMapView);
        }
    }
}