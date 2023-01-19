using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OrderElimination.BattleMap;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityView
    {
        public event Action Casted;

        private readonly Ability _ability;
        private readonly BattleMapView _battleMapView;
        private readonly BattleCharacterView _casterView;
        private readonly int _abilityDistance;

        private int _coolDownTimer;

        private bool _casting;

        private List<CellView> _selectedCellViews = new();

        public AbilityInfo AbilityInfo { get; }

        public IActor Caster { get; }

        public Sprite AbilityIcon => AbilityInfo.Icon;

        public bool CanCast => _coolDownTimer <= 0 && !_casting && Caster.CanSpendAction(AbilityInfo.ActionType);

        public string Name => AbilityInfo.Name;

        private CancellationTokenSource _cancellationTokenSource;

        public AbilityView(IActor caster, Ability ability, AbilityInfo info, BattleMapView battleMapView,
            BattleCharacterView casterView)
        {
            Caster = caster;
            _ability = ability;
            _battleMapView = battleMapView;
            _casterView = casterView;

            _casterView.FireLine.gameObject.SetActive(false);

            AbilityInfo = info;
            _abilityDistance = AbilityInfo.ActiveParams.DistanceFromMovement
                ? Caster.Stats.Movement
                : AbilityInfo.ActiveParams.Distance;
            _coolDownTimer = AbilityInfo.StartCoolDown;

            BattleSimulation.RoundStarted += OnRoundStart;
            BattleSimulation.BattleEnded += OnBattleEnded;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async void Clicked()
        {
            if (_casting)
            {
                CancelCast();
                return;
            }
            _battleMapView.DelightCells();
            if (!Caster.CanSpendAction(AbilityInfo.ActionType))
            {
                NotEnoughActions();
                return;
            }

            if (_coolDownTimer > 0)
            {
                Debug.LogWarning("Ability is on cooldown");
                return;
            }

            LightTargets();

            if (AbilityInfo.ActionType != ActionType.Movement)
            {
                _casterView.FireLine.gameObject.SetActive(true);
                _casterView.FireLine.SetPosition(1, _casterView.transform.position);
                _casterView.FireLine.SetPosition(0, _casterView.transform.position);
            }

            var success = await TryCast();
            _casterView.FireLine.gameObject.SetActive(false);
            if (!success)
                return;
            _battleMapView.DelightCells();
        }

        private static void NotEnoughActions()
        {
            Debug.LogWarning("Dont enough actions");
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
            var casterCoords = _battleMapView.Map.GetCoordinate(Caster);
            _battleMapView.LightCellByDistance(casterCoords.x, casterCoords.y, _abilityDistance);
        }

        //TODO: Refactor with async abilities
        private async UniTask<bool> TryCast()
        {
            if (_casting)
                return false;
            _casting = true;
            var targetSelection = await SelectTarget()
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();
            if (targetSelection.IsCanceled)
            {
                foreach (var selectedObj in _selectedCellViews.Select(cell => cell.Model.GetObject()))
                {
                    if (selectedObj is not NullBattleObject &&
                        selectedObj.View.TryGetComponent(out BattleCharacterView view))
                        view.HideAccuracy();
                }

                _selectedCellViews.Clear();
                return true;
            }

            IBattleObject target = targetSelection.Result;

            if (!Caster.TrySpendAction(AbilityInfo.ActionType))
                throw new Exception("Dont enough actions");
            _ability.Use(target, Caster.Stats);

            _coolDownTimer = AbilityInfo.CoolDown;
            Casted?.Invoke();

            _casting = false;
            return true;
        }

        private async UniTask<IBattleObject> SelectTarget()
        {
            IBattleObject target = null;
            _casting = true;

            var cellConfirmed = false;
            List<IBattleObject> availableTargets = GetTargets();

            void OnCellClicked(CellView cell)
            {
                IBattleObject selected = cell.Model.GetObject();
                if (!availableTargets.Contains(selected))
                {
                    WrongTargetSelected();
                    return;
                }

                DeselectCells(_selectedCellViews);
                _selectedCellViews.Clear();

                if (selected == target)
                {
                    cellConfirmed = true;
                    return;
                }

                if (AbilityInfo.ActiveParams.HasAreaEffect)
                {
                    var area = _battleMapView.Map.GetBattleObjectsInRadius(selected,
                        AbilityInfo.ActiveParams.AreaRadius, AbilityInfo.ActiveParams.LightTargetsSide);
                    foreach (var obj in area)
                    {
                        var areaCell = _battleMapView.GetCell(obj);
                        areaCell.Select();
                        _selectedCellViews.Add(areaCell);
                    }
                }

                cell.Select();
                _selectedCellViews.Add(cell);

                target = selected;
                _casterView.FireLine.SetPosition(1, cell.transform.position);

                foreach (var selectedObj in _selectedCellViews.Select(selectedCellView =>
                             selectedCellView.Model.GetObject()))
                {
                    if (selectedObj is NullBattleObject || selectedObj == Caster ||
                        !selectedObj.View.TryGetComponent(out BattleCharacterView view))
                        continue;
                    var accuracy = selectedObj.GetAccuracyFrom(Caster);
                    view.ShowAccuracy(accuracy);
                }
            }

            _battleMapView.CellClicked += OnCellClicked;

            if (AbilityInfo.ActiveParams.TargetType == TargetType.Self)
                OnCellClicked(_battleMapView.GetCell(Caster));

            await UniTask.WaitUntil(() => cellConfirmed)
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();

            _battleMapView.CellClicked -= OnCellClicked;

            return target;
        }

        private static void DeselectCells(List<CellView> selectedCellViews)
        {
            foreach (var cell in selectedCellViews)
            {
                cell.Deselect();
                var selectedObj = cell.Model.GetObject();
                if (selectedObj is not NullBattleObject &&
                    selectedObj.View.TryGetComponent(out BattleCharacterView view))
                    view.HideAccuracy();
            }
        }

        private void WrongTargetSelected()
        {
            Debug.LogWarning("Wrong target selected");
        }

        private List<IBattleObject> GetTargets()
        {
            List<IBattleObject> targets = new List<IBattleObject>();
            switch (AbilityInfo.ActiveParams.TargetType)
            {
                case TargetType.Self:
                    targets.Add(Caster);
                    break;
                case TargetType.Empty:
                    targets.AddRange(_battleMapView.Map.GetEmptyObjectsInRadius(Caster, _abilityDistance));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectSide.Environment));
                    break;
                case TargetType.Enemy:
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectSide.Enemy));
                    break;
                case TargetType.Ally:
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectSide.Ally));
                    targets.Add(Caster);
                    break;
                case TargetType.All:
                    targets.AddRange(_battleMapView.Map.GetEmptyObjectsInRadius(Caster, _abilityDistance));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectSide.Enemy));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectSide.Ally));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectSide.Environment));
                    targets.Add(Caster);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return targets;
        }

        private void OnRoundStart()
        {
            _coolDownTimer--;
            Casted?.Invoke();
        }

        private void OnBattleEnded(BattleOutcome outcome)
        {
            CancelCast();
            _cancellationTokenSource.Cancel();
        }
    }
}