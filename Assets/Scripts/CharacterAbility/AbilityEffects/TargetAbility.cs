namespace CharacterAbility.AbilityEffects
{
    public class TargetAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _distance;
        private readonly bool _selfCast;

        public TargetAbility(BattleCharacter caster, Ability nextAbility, int distance, bool selfCast) 
            : base(caster)
        {
            _nextAbility = nextAbility;
            _distance = distance;
            _selfCast = selfCast;
        }

        public override async void Use(IBattleObject target, BattleMapView battleMapView)
        {
            var castPos = battleMapView.Map.GetCoordinate(_caster);
            battleMapView.LightCellByDistance(castPos.x, castPos.y, _distance);
            //await UniTask.WaitUntil(() => Input.); TODO: Ожидание клика по клетке
            _nextAbility.Use(_selfCast ? _caster : target, battleMapView);
        }
    }
}