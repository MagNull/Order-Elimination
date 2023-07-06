using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class AbilityPreviewDisplayer : MonoBehaviour//Only for active abilities
{
    [SerializeField]
    private DamageActionDisplay _damageDisplayPrefab;
    [SerializeField]
    private HealActionDisplay _healDisplayPrefab;

    private ObjectPool<DamageActionDisplay> _damageDisplaysPool;
    private ObjectPool<HealActionDisplay> _healDisplaysPool;
    private Dictionary<Vector2Int, ActionCellDisplay> _cellDisplays = new();

    private void Awake()
    {
        _damageDisplaysPool = new ObjectPool<DamageActionDisplay>(
            () => Instantiate(_damageDisplayPrefab, transform),
            d => d.gameObject.SetActive(true),
            d => d.gameObject.SetActive(false));
        _healDisplaysPool = new ObjectPool<HealActionDisplay>(
            () => Instantiate(_healDisplayPrefab, transform),
            d => d.gameObject.SetActive(true),
            d => d.gameObject.SetActive(false));
    }

    public void DisplayPreview(IActiveAbilityData ability, AbilitySystemActor caster, CellGroupsContainer targetedGroups)
    {
        HidePreview();
        foreach (var i in ability.Execution.ActionInstructions)
            DisplayInstruction(i, caster, targetedGroups);
    }

    public void HidePreview()
    {
        foreach (var pos in _cellDisplays.Keys)
        {
            foreach (var display in _cellDisplays[pos].DamageDisplays)
            {
                _damageDisplaysPool.Release(display);
            }
            foreach (var display in _cellDisplays[pos].HealDisplays)
            {
                _healDisplaysPool.Release(display);
            }
            _cellDisplays[pos].DamageDisplays.Clear();
            _cellDisplays[pos].HealDisplays.Clear();
        }
        _cellDisplays.Clear();
    }

    private void DisplayInstruction(AbilityInstruction instruction, AbilitySystemActor caster, CellGroupsContainer targetedGroups)
    {
        var battleContext = caster.BattleContext;
        var repSuffix = instruction.RepeatNumber > 1 ? $" x {instruction.RepeatNumber}" : "";
        foreach (var pos in targetedGroups
                .ContainedCellGroups
                .Where(gId => instruction.AffectedCellGroups.Contains(gId))
                .SelectMany(gId => targetedGroups.GetGroup(gId)))
        {
            var visualPosition = battleContext.AnimationSceneContext.BattleMapView.GameToWorldPosition(pos);
            foreach (var target in battleContext
                .GetVisibleEntities(pos, caster.BattleSide)
                .Where(e => instruction.TargetConditions.All(c => c.IsConditionMet(battleContext, caster, e))))
            {
                var actionContext = new ActionContext(battleContext, targetedGroups, caster, target);
                if (instruction.Action is InflictDamageAction damageAction)
                {
                    damageAction = damageAction.GetModifiedAction(actionContext);
                    var display = _damageDisplaysPool.Get();
                    display.transform.position = visualPosition;
                    display.DamageValue = $"{damageAction.DamageSize.GetValue(actionContext)}{repSuffix}";
                    display.AccuracyValue = $"{Mathf.RoundToInt(damageAction.Accuracy.GetValue(actionContext) * 100)} %";
                    if (!_cellDisplays.ContainsKey(pos))
                        _cellDisplays.Add(pos, new());
                    else
                        Debug.LogError("Placing multiple action displays on the same position.");
                    _cellDisplays[pos].DamageDisplays.Add(display);
                }
                if (instruction.Action is HealAction healAction)
                {
                    healAction = healAction.GetModifiedAction(actionContext);
                    var display = _healDisplaysPool.Get();
                    display.transform.position = visualPosition;
                    var healInfo = new RecoveryInfo(
                        healAction.HealSize.GetValue(actionContext),
                        healAction.ArmorMultiplier,
                        healAction.HealthMultiplier,
                        healAction.HealPriority,
                        caster);
                    var availableHeal = IBattleLifeStats.DistributeRecovery(target.BattleStats, healInfo);
                    display.HealValue = $"+{availableHeal.TotalRecovery}{repSuffix}";
                    if (!_cellDisplays.ContainsKey(pos))
                        _cellDisplays.Add(pos, new());
                    else
                        Debug.LogError("Placing multiple action displays on the same position.");
                    _cellDisplays[pos].HealDisplays.Add(display);
                }
            }
        }
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

public class ActionCellDisplay
{
    public readonly List<DamageActionDisplay> DamageDisplays = new();
    public readonly List<HealActionDisplay> HealDisplays = new();
}
