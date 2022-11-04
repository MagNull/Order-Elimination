using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class MoveAbility : Ability
    {
        private readonly Ability _nextAbility;

        public MoveAbility(BattleCharacter caster, Ability nextAbility) : base(caster)
        {
            _nextAbility = nextAbility;
        }

        public override async void Use(IBattleObject target, BattleMapView battleMapView)
        {
            if (target == null)
            {
                var availableObjects =
                    battleMapView.Map.GetEmptyObjectsInRadius(_caster, _caster.GetStats().Movement);
                Debug.Log(availableObjects.Count);
                battleMapView.Map.CellSelected += c => target = c.GetObject();
                await UniTask.WaitUntil(() => target != null && availableObjects.Contains(target));
            }

            var point = battleMapView.Map.GetCoordinate(target);
            battleMapView.Map.MoveTo(_caster, point.x, point.y);
            _nextAbility?.Use(target, battleMapView);
        }
    }
}