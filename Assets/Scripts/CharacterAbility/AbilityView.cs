using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityView
    {
        public event Action Casted;

        private readonly Ability _ability;
        private readonly BattleMapView _battleMapView;
        private readonly int _abilityDistance;

        private int _coolDownTimer;

        private bool _casting;

        public AbilityInfo AbilityInfo { get; }

        public IAbilityCaster Caster { get; }

        public Sprite AbilityIcon => AbilityInfo.Icon;

        public bool CanCast => _coolDownTimer <= 0 && !_casting && Caster.CanSpendAction(AbilityInfo.ActionType);

        public string Name => AbilityInfo.Name;

        private CancellationTokenSource _cancellationTokenSource;

        public AbilityView(IAbilityCaster caster, Ability ability, AbilityInfo info, BattleMapView battleMapView)
        {
            Caster = caster;
            _ability = ability;
            _battleMapView = battleMapView;
            AbilityInfo = info;
            _abilityDistance = AbilityInfo.DistanceFromMovement ? Caster.Stats.Movement : AbilityInfo.Distance;
            _coolDownTimer = AbilityInfo.StartCoolDown;

            BattleSimulation.RoundStarted += OnRoundStart;
            BattleSimulation.BattleEnded += OnBattleEnded;    
            
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async void Clicked()
        {
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
            var success = await TryCast();
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

        private async UniTask<bool> TryCast()
        {
            if (_casting)
                return false;
            _casting = true;
            var targetSelection = await SelectTarget()
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();
            if (targetSelection.IsCanceled)
                return true;

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
            List<CellView> selectedCellViews = new List<CellView>();

            void OnCellClicked(CellView cell)
            {
                IBattleObject selected = cell.Model.GetObject();
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

                if (AbilityInfo.HasAreaEffect)
                {
                    var area = _battleMapView.Map.GetBattleObjectsInRadius(selected, AbilityInfo.AreaRadius);
                    foreach (var obj in area)
                    {
                        var areaCell = _battleMapView.GetCell(obj);
                        areaCell.Select();
                        selectedCellViews.Add(areaCell);
                    }
                }

                cell.Select();
                selectedCellViews.Add(cell);

                target = selected;
            }

            _battleMapView.CellClicked += OnCellClicked;

            if (AbilityInfo.TargetType == TargetType.Self)
                OnCellClicked(_battleMapView.GetCell(Caster));

            await UniTask.WaitUntil(() => cellConfirmed)
                .AttachExternalCancellation(_cancellationTokenSource.Token)
                .SuppressCancellationThrow();

            _battleMapView.CellClicked -= OnCellClicked;

            return target;
        }

        private void WrongTargetSelected()
        {
            Debug.LogWarning("Wrong target selected");
        }

        private List<IBattleObject> GetTargets()
        {
            List<IBattleObject> targets = new List<IBattleObject>();
            switch (AbilityInfo.TargetType)
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