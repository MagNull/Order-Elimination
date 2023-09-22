using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class AbilityPreviewDisplayer : MonoBehaviour//Only for active abilities
{
    [Header("Setup")]
    [SerializeField]
    private MultilineActionDisplay _actionDisplayPrefab;
    [SerializeField, PreviewField(Alignment = ObjectFieldAlignment.Left)]
    private Sprite _damageIcon;
    [SerializeField, PreviewField(Alignment = ObjectFieldAlignment.Left)]
    private Sprite _accuracyIcon;
    [SerializeField, PreviewField(Alignment = ObjectFieldAlignment.Left)]
    private Sprite _armorIcon;
    [SerializeField, PreviewField(Alignment = ObjectFieldAlignment.Left)]
    private Sprite _healthIcon;
    [Header("Setup")]
    [SerializeField]
    private bool _accuracyAffectionAsFormulas;
    [SerializeField]
    private bool _roundFloatNumbers;
    [SerializeField, ShowIf(nameof(_roundFloatNumbers))]
    private RoundingOption _roundingMode;

    private ObjectPool<MultilineActionDisplay> _displaysPool;
    private Dictionary<Vector2Int, MultilineActionDisplay> _displaysInCells = new();
    private BattleMapView _battleMapView;

    [Inject]
    private void Construct(BattleMapView mapView)
    {
        _battleMapView = mapView;
    }

    private void Awake()
    {
        _displaysPool = new ObjectPool<MultilineActionDisplay>(
            () => Instantiate(_actionDisplayPrefab, transform),
            d => 
            { 
                d.gameObject.SetActive(true);
                d.ClearParameters();
            },
            d =>
            {
                d.gameObject.SetActive(false);
            },
            d => Destroy(d.gameObject));
    }

    public void DisplayPreview(IActiveAbilityData ability, AbilitySystemActor caster, CellGroupsContainer targetedGroups)
    {
        HidePreview();
        foreach (var i in ability.Execution.ActionInstructions)
            DisplayInstruction(i, caster, targetedGroups);
    }

    public void HidePreview()
    {
        foreach (var display in _displaysInCells.Values)
        {
            _displaysPool.Release(display);
        }
        _displaysInCells.Clear();
    }

    private MultilineActionDisplay GetDisplayForCell(Vector2Int position)
    {
        MultilineActionDisplay display;
        if (!_displaysInCells.ContainsKey(position))
        {
            display = _displaysPool.Get();
            _displaysInCells.Add(position, display);
        }
        else
            display = _displaysInCells[position];
        display.transform.position = _battleMapView.GameToWorldPosition(position);
        return display;
    }

    private void DisplayInstruction(AbilityInstruction instruction, AbilitySystemActor caster, CellGroupsContainer targetedGroups)
    {
        //prev target affection?
        var battleContext = caster.BattleContext;
        var casterPos = caster.Position;
        var repSuffix = instruction.RepeatNumber > 1 ? $" x {instruction.RepeatNumber}" : "";
        foreach (var pos in targetedGroups
                .ContainedCellGroups
                .Where(gId => instruction.AffectedCellGroups.Contains(gId))
                .SelectMany(gId => targetedGroups.GetGroup(gId)))
        {
            var visualPosition = battleContext.AnimationSceneContext.BattleMapView.GameToWorldPosition(pos);
            foreach (var target in battleContext
                .GetVisibleEntitiesAt(pos, caster.BattleSide)
                .Where(e => instruction.TargetConditions.AllMet(battleContext, caster, e)))
            {
                var actionContext = new ActionContext(battleContext, targetedGroups, caster, target);
                var valueContext = ValueCalculationContext.Full(actionContext);
                if (instruction.Action is InflictDamageAction damageAction)
                {
                    var modifiedAction = damageAction.GetModifiedAction(actionContext);
                    var display = GetDisplayForCell(pos);
                    var accuracyValue = Mathf.Max(
                        0, modifiedAction.Accuracy.GetValue(valueContext) * 100);
                    var damage = modifiedAction.CalculateDamage(actionContext);
                    var distributedDamage = IBattleLifeStats.DistributeDamage(target.BattleStats, damage);
                    float damageValue = distributedDamage.TotalDamage;
                    if (_roundFloatNumbers)
                    {
                        damageValue = MathExtensions.Round(damageValue, _roundingMode);
                        accuracyValue = MathExtensions.Round(accuracyValue, _roundingMode);
                    }
                    display.AddParameter(_damageIcon, Color.red, $"{damageValue}{repSuffix}");
                    display.AddParameter(_accuracyIcon, Color.red, $"{accuracyValue} %");
                    if (modifiedAction.ObjectsBetweenAffectAccuracy 
                        && caster != null)
                    {
                        var intersections = CellMath.GetIntersectionBetween(casterPos, pos);
                        var modifiedAccuracy = damageAction.Accuracy.Clone();
                        var previousCellValue = modifiedAccuracy.GetValue(valueContext);
                        foreach (var intersection in intersections)
                        {
                            var intPos = intersection.CellPosition;
                            var isModified = false;
                            foreach (var obstacle in battleContext
                                .GetVisibleEntitiesAt(intPos, caster.BattleSide)
                                .Select(e => e.Obstacle))
                            {
                                var modification = obstacle.ModifyAccuracy(
                                    modifiedAccuracy,
                                    intersection.IntersectionAngle,
                                    intersection.SmallestPartSquare,
                                    caster);
                                if (modification.IsModificationSuccessful)
                                {
                                    modifiedAccuracy = modification.ModifiedValueGetter;
                                    battleContext.EntitiesBank
                                        .GetViewByEntity(obstacle.ObstacleEntity)
                                        .Highlight(Color.red);
                                    isModified = true;
                                }
                            }
                            if (isModified)
                            {
                                var accuracyDisplay = GetDisplayForCell(intPos);
                                //var initialValue = damageAction.Accuracy.GetValue(actionContext);
                                var modifiedValue = modifiedAccuracy.GetValue(valueContext);
                                var affection = (modifiedValue - previousCellValue) * 100;
                                if (_roundFloatNumbers)
                                    affection = MathExtensions.Round(affection, _roundingMode);
                                var displayedAffection = $"{(affection >= 0 ? "+" : "")}{affection}%";
                                if (_accuracyAffectionAsFormulas)
                                    displayedAffection = modifiedAccuracy.DisplayedFormula;
                                accuracyDisplay.AddParameter(_accuracyIcon, Color.red, displayedAffection);
                            }
                            previousCellValue = modifiedAccuracy.GetValue(valueContext);
                        }
                    }
                }
                if (instruction.Action is HealAction healAction)
                {
                    healAction = healAction.GetModifiedAction(actionContext);
                    var display = GetDisplayForCell(pos);
                    var healInfo = new RecoveryInfo(
                        healAction.HealSize.GetValue(valueContext),
                        healAction.ArmorMultiplier,
                        healAction.HealthMultiplier,
                        healAction.HealPriority,
                        caster);
                    var availableHeal = IBattleLifeStats.DistributeRecovery(target.BattleStats, healInfo);
                    display.AddParameter(
                        _armorIcon,
                        $"+{availableHeal.TotalArmorRecovery}{repSuffix}");
                    display.AddParameter(
                        _healthIcon,
                        $"+{availableHeal.TotalHealthRecovery}{repSuffix}");
                }
            }
        }
        //Next instructions
        foreach (var sucInstruction in instruction.InstructionsOnActionSuccess)
        {
            DisplayInstruction(sucInstruction, caster, targetedGroups);
        }
        foreach (var failInstruction in instruction.InstructionsOnActionFail)
        {
            DisplayInstruction(failInstruction, caster, targetedGroups);
        }
        foreach (var followInstruction in instruction.FollowingInstructions)
        {
            DisplayInstruction(followInstruction, caster, targetedGroups);
        }
    }
}
