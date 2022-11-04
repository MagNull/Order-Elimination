using Cysharp.Threading.Tasks;

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
            if (target == null)
            {
                var availableObjects =
                    battleMapView.Map.GetBattleObjectsInRadius(_caster, _distance);
                battleMapView.Map.CellSelected += c => target = c.GetObject();
                await UniTask.WaitUntil(() => target != null && availableObjects.Contains(target));
            }
            var castPos = battleMapView.Map.GetCoordinate(_caster);
            battleMapView.LightCellByDistance(castPos.x, castPos.y, _distance);
            //await UniTask.WaitUntil(() => Input.); TODO: Ожидание клика по клетке
            _nextAbility.Use(_selfCast ? _caster : target, battleMapView);
        }
    }
}