using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CharacterAbility.AbilityEffects;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterAbility
{
    public class AbilityView
    {
        private readonly Sprite _abilityIcon;
        private readonly BattleCharacter _caster;
        [ShowInInspector]
        private readonly Ability _ability;
        private readonly BattleMapView _battleMapView;
        private readonly AbilityInfo _abilityInfo;
        private readonly int _abilityDistance;

        private bool _selected;
        public Sprite AbilityIcon => _abilityIcon;

        public AbilityView(BattleCharacter caster, Ability ability, AbilityInfo info, BattleMapView battleMapView)
        {
            _caster = caster;
            _ability = ability;
            _battleMapView = battleMapView;
            _abilityInfo = info;
            _abilityIcon = info.Icon;
            _abilityDistance = _abilityInfo.DistanceFromMovement ? _caster.Stats.Movement : _abilityInfo.Distance;
        }

        public async void Clicked()
        {
            LightTargets();
            await Cast();
        }

        private void LightTargets()
        {
            var casterCoords = _battleMapView.Map.GetCoordinate(_caster);
            _battleMapView.LightCellByDistance(casterCoords.x, casterCoords.y, _abilityDistance);
        }

        private async UniTask Cast()
        {
            if (_ability == null || _selected)
                return;
            IBattleObject target = null;
            _selected = true;

            var availableTargets = GetTargets();
            _battleMapView.Map.CellSelected += cell => target = cell.GetObject();

            await UniTask.WaitUntil(() => availableTargets.Contains(target));

            _ability.Use(target, _battleMapView.Map);
            _selected = false;
        }

        private List<IBattleObject> GetTargets()
        {
            List<IBattleObject> targets = new List<IBattleObject>();
            switch (_abilityInfo.TargetType)
            {
                case TargetType.Self:
                    targets.Add(_caster);
                    break;
                case TargetType.Empty:
                    targets.AddRange(_battleMapView.Map.GetEmptyObjectsInRadius(_caster, _abilityDistance));
                    break;
                case TargetType.Enemy:
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(_caster, _abilityDistance,
                        BattleObjectSide.Enemy));
                    break;
                case TargetType.Ally:
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(_caster, _abilityDistance,
                        BattleObjectSide.Player));
                    break;
                case TargetType.All:
                    targets.AddRange(_battleMapView.Map.GetEmptyObjectsInRadius(_caster, _abilityDistance));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(_caster, _abilityDistance,
                        BattleObjectSide.Enemy));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(_caster, _abilityDistance,
                        BattleObjectSide.Player));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return targets;
        }
    }
}