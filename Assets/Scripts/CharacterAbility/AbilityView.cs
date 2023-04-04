using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OrderElimination.BM;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityView
    {
        public event Action<ActionType> Casted;

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

        public bool Casting => _casting;

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

            BattleSimulation.PlayerTurnStarted += OnPlayerTurnStart;
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
                foreach (var objs in _selectedCellViews)
                {
                    if (objs.Model.Contains(obj => obj is BattleCharacter, out var selectedObj) &&
                        selectedObj.View.GameObject.TryGetComponent(out BattleCharacterView view))
                        view.HideAccuracy();
                }

                _selectedCellViews.Clear();
                return true;
            }

            IReadOnlyList<IBattleObject> targets = targetSelection.Result;

            if (!Caster.TrySpendAction(AbilityInfo.ActionType))
                throw new Exception("Dont enough actions");
            foreach (var target in targets) 
                _ability.Use(target, Caster.Stats);

            _coolDownTimer = AbilityInfo.CoolDown;
            //TODO: Rework crutch
            if (!AbilityInfo.NotTriggerCast)
            {
                Caster.OnCasted(AbilityInfo.ActionType);
            }

            Casted?.Invoke(AbilityInfo.ActionType);

            _casting = false;
            return true;
        }

        private async UniTask<IReadOnlyList<IBattleObject>> SelectTarget()
        {
            IReadOnlyCell target = null;
            _casting = true;

            var cellConfirmed = false;
            List<IBattleObject> availableTargets = GetTargets();
            Debug.Log(availableTargets.Count);

            void OnCellClicked(CellView cellView)
            {
                IReadOnlyCell selectedCell = cellView.Model;
                if (!selectedCell.Objects.Any(obj => availableTargets.Contains(obj)))
                {
                    if (AbilityInfo.ActionType != ActionType.Movement)
                        WrongTargetSelected();
                    return;
                }

                DeselectCells(_selectedCellViews);
                _selectedCellViews.Clear();

                if (selectedCell == target)
                {
                    cellConfirmed = true;
                    return;
                }

                void SelectSeveral(IList<IBattleObject> battleObjects)
                {
                    foreach (var obj in battleObjects)
                    {
                        var areaCell = _battleMapView.GetCell(obj);
                        areaCell.Select();
                        _selectedCellViews.Add(areaCell);
                    }
                }

                if (AbilityInfo.ActiveParams.HasAreaEffect)
                {
                    var area = _battleMapView.Map.GetBattleObjectsInRadius(selectedCell.Objects[0],
                        AbilityInfo.ActiveParams.AreaRadius, AbilityInfo.ActiveParams.LightTargetsType);
                    SelectSeveral(area);
                }

                if (AbilityInfo.ActiveParams.HasPatternTargetEffect)
                {
                    var pattern = _battleMapView.Map.GetBattleObjectsInPatternArea(selectedCell.Objects[0],
                        Caster, AbilityInfo.ActiveParams.Pattern, AbilityInfo.ActiveParams.LightTargetsType,
                        AbilityInfo.ActiveParams.PatternMaxDistance);
                    SelectSeveral(pattern);
                }

                cellView.Select();
                _selectedCellViews.Add(cellView);

                target = selectedCell;
                _casterView.FireLine.SetPosition(1, cellView.transform.position);

                foreach (var cell in _selectedCellViews.Select(selectedCellView =>
                             selectedCellView.Model))
                {
                    if (!cell.Contains(o => o is BattleCharacter, out var selectedObj))
                        continue;
                    var accuracy = selectedObj.GetAccuracyFrom(Caster);
                    selectedObj.View.ShowAccuracy(accuracy);
                }
            }

            _battleMapView.CellClicked += OnCellClicked;

            if (AbilityInfo.ActiveParams.TargetType == TargetType.Self)
                OnCellClicked(_battleMapView.GetCell(Caster));

            await UniTask.WaitUntil(() => cellConfirmed)
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();

            _battleMapView.CellClicked -= OnCellClicked;

            return target.Objects;
        }

        private static void DeselectCells(List<CellView> selectedCellViews)
        {
            foreach (var cell in selectedCellViews)
            {
                cell.Deselect();
                var selectedCell = cell.Model;
                if (selectedCell.Contains(obj => obj is BattleCharacter, out var character))
                    character.View.HideAccuracy();
            }
        }

        private void WrongTargetSelected()
        {
            _casterView.EmmitText("Wrong target", Color.red, .8f); //TODO: Fix magic number
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
                        BattleObjectType.Environment));
                    break;
                case TargetType.Enemy:
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectType.Enemy));
                    break;
                case TargetType.Ally:
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectType.Ally));
                    targets.Add(Caster);
                    break;
                case TargetType.All:
                    targets.AddRange(_battleMapView.Map.GetEmptyObjectsInRadius(Caster, _abilityDistance));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectType.Enemy));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectType.Ally));
                    targets.AddRange(_battleMapView.Map.GetBattleObjectsInRadius(Caster, _abilityDistance,
                        BattleObjectType.Environment));
                    targets.Add(Caster);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return targets;
        }

        private void OnPlayerTurnStart()
        {
            _coolDownTimer--;
            Casted?.Invoke(AbilityInfo.ActionType);
        }

        private void OnBattleEnded(BattleOutcome outcome)
        {
            CancelCast();
            _cancellationTokenSource.Cancel();
        }
    }
}