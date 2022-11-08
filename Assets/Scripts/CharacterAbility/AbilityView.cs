using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityView
    {
        private readonly BattleCharacter _caster;
        private readonly Ability _ability;
        private readonly BattleMapView _battleMapView;
        private readonly AbilityInfo _abilityInfo;
        private readonly int _abilityDistance;

        private int _coolDownTimer;

        private bool _casting;
        public Sprite AbilityIcon => _abilityInfo.Icon;

        private CancellationTokenSource _cancellationTokenSource;

        public AbilityView(BattleCharacter caster, Ability ability, AbilityInfo info, BattleMapView battleMapView)
        {
            _caster = caster;
            _ability = ability;
            _battleMapView = battleMapView;
            _abilityInfo = info;
            _abilityDistance = _abilityInfo.DistanceFromMovement ? _caster.Stats.Movement : _abilityInfo.Distance;
            _coolDownTimer = _abilityInfo.StartCoolDown;

            BattleSimulation.RoundStarted += OnRoundStart;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async void Clicked()
        {
            _battleMapView.DelightCells();
            if (!_caster.AvailableActions.Contains(_abilityInfo.ActionType))
            {
                Debug.LogWarning("Dont enough actions");
                return;
            }

            if (_coolDownTimer > 0)
            {
                Debug.LogWarning("Ability is on cooldown");
                return;
            }

            LightTargets();
            await Cast();
            _battleMapView.DelightCells();
        }

        public void CancelCast()
        {
            _battleMapView.DelightCells();
            _casting = false;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void LightTargets()
        {
            var casterCoords = _battleMapView.Map.GetCoordinate(_caster);
            _battleMapView.LightCellByDistance(casterCoords.x, casterCoords.y, _abilityDistance);
        }

        private async UniTask Cast()
        {
            if (_casting)
                return;
            _casting = true;
            var targetSelection = await SelectTarget()
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();
            if (targetSelection.IsCanceled)
                return;

            IBattleObject target = targetSelection.Result;

            if (!_caster.TrySpendAction(_abilityInfo.ActionType))
                throw new Exception("Dont enough actions");
            _ability.Use(target, _caster.Stats, _battleMapView.Map);

            _coolDownTimer = _abilityInfo.CoolDown;

            _casting = false;
        }

        private async UniTask<IBattleObject> SelectTarget()
        {
            IBattleObject target = null;
            _casting = true;

            var cellConfirmed = false;
            List<IBattleObject> availableTargets = GetTargets();
            List<CellView> selectedCellViews = new List<CellView>();

            void OnCellClicked(CellView cell)
            {
                IBattleObject selected = cell.GetObject();
                if (!availableTargets.Contains(selected))
                {
                    WrongTargetSelected();
                    return;
                }

                selectedCellViews.ForEach(c => c.Deselect());
                selectedCellViews.Clear();

                if (selected == target)
                {
                    cellConfirmed = true;
                    selectedCellViews.ForEach(c => c.Deselect());
                    return;
                }

                if (_abilityInfo.HasAreaEffect)
                {
                    var area = _battleMapView.Map.GetBattleObjectsInRadius(selected, _abilityInfo.AreaRadius);
                    foreach (var obj in area)
                    {
                        var objCoords = _battleMapView.Map.GetCoordinate(obj);
                        var areaCell = _battleMapView.Map.GetCell(objCoords.x, objCoords.y);
                        areaCell.Select();
                        selectedCellViews.Add(areaCell);
                    }
                }

                cell.Select();
                selectedCellViews.Add(cell);

                target = selected;
            }

            _battleMapView.Map.CellClicked += OnCellClicked;

            await UniTask.WaitUntil(() => cellConfirmed)
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();
            
            
            _battleMapView.Map.CellClicked -= OnCellClicked;

            return target;
        }

        private void WrongTargetSelected()
        {
            Debug.LogWarning("Wrong target selected");
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

        private void OnRoundStart()
        {
            _coolDownTimer--;
        }
    }
}