using System;
using System.Collections.Generic;
using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination;
using OrderElimination.Battle;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ActionType
{
    Movement,
    Ability,
    Difficult,
    Free
}

[Serializable]
public class BattleCharacter : IActor
{
    public event Action<TakeDamageInfo> Damaged;
    public event Action<Cell, Cell> Moved;
    public event Action<BattleCharacter> Died;

    [ShowInInspector]
    private readonly List<ITickEffect> _tickEffects;
    [ShowInInspector]
    private readonly List<IncomingBuff> _incomingTickEffects;
    [ShowInInspector]
    private readonly List<IStatsBuffEffect> _buffEffects;
    [ShowInInspector]
    private readonly BattleObjectSide _side;
    [SerializeField]
    private BattleStats _battleStats;
    private IDamageCalculation _damageCalculation;

    [ShowInInspector]
    private readonly ActionBank _actionBank;

    public BattleObjectSide Side => _side;
    public GameObject View { get; set; }

    public IReadOnlyList<ActionType> AvailableActions => _actionBank.AvailableActions;

    public IReadOnlyBattleStats Stats => _battleStats;

    public event Action<ITickEffect> EffectAdded;

    public BattleCharacter(BattleObjectSide side, BattleStats battleStats, IDamageCalculation damageCalculation)
    {
        _damageCalculation = damageCalculation;
        _side = side;
        _battleStats = battleStats;
        _tickEffects = new List<ITickEffect>();
        _buffEffects = new List<IStatsBuffEffect>();
        _incomingTickEffects = new List<IncomingBuff>();
        _actionBank = new ActionBank();
    }

    public void OnMoved(Cell from, Cell to) => Moved?.Invoke(from, to);

    public void TakeDamage(DamageInfo damageInfo)
    {
        var damageTaken =
            _damageCalculation.CalculateDamage(damageInfo, _battleStats.Armor,
                _battleStats.Evasion, _incomingTickEffects);
        var takeDamageInfo = new TakeDamageInfo
        {
            HealthDamage = damageTaken.healthDamage,
            ArmorDamage = damageTaken.armorDamage,
            CancelType = damageTaken.cancelType,
            Attacker = damageInfo.Attacker,
            Target = this
        };
        //TODO Логика ивента при уроне
        _battleStats.Armor -= damageTaken.armorDamage;
        _battleStats.Health -= damageTaken.healthDamage;
        Damaged?.Invoke(takeDamageInfo);

        if (_battleStats.Health > 0) return;
        _battleStats.Health = 0;
        Died?.Invoke(this);
    }

    //TODO: Strategy pattern in future if needed
    public void TakeRecover(int value, int accuracy, DamageHealTarget damageHealTarget)
    {
        var isHeal = Random.Range(0, 100) < accuracy;
        if (!isHeal)
            return;

        switch (damageHealTarget)
        {
            case DamageHealTarget.OnlyHealth:
                _battleStats.Health += value;
                break;
            case DamageHealTarget.OnlyArmor:
                _battleStats.Armor += value;
                break;
            case DamageHealTarget.Normal:
            default:
                _battleStats.Health += value;
                break;
        }
    }

    public void ClearTickEffects()
    {
        _tickEffects.Clear();
    }

    public int GetAccuracyFrom(IBattleObject attacker)
    {
        var accuracy = attacker.Stats.Accuracy;
        foreach (var effect in _incomingTickEffects)
        {
            accuracy = effect.GetModifiedValue(accuracy, Buff_Type.IncomingAccuracy);
        }

        accuracy = Mathf.Clamp(accuracy, 0, 100);
        return accuracy;
    }

    public void OnTurnStart()
    {
        TickEffects();
        RefreshActions();
    }

    public void AddTickEffect(ITickEffect effect)
    {
        switch (effect)
        {
            case IncomingBuff incomingDebuff:
                _incomingTickEffects.Add(incomingDebuff);
                break;
            case IStatsBuffEffect statsBuffEffect:
                _buffEffects.Add(statsBuffEffect);
                _battleStats = statsBuffEffect.Apply(this);
                break;
            default:
                _tickEffects.Add(effect);
                break;
        }

        EffectAdded?.Invoke(effect);
    }

    public virtual void PlayTurn()
    {
    }

    public void RemoveTickEffect(ITickEffect effect)
    {
        switch (effect)
        {
            case IncomingBuff incomingDebuff:
                _incomingTickEffects.Remove(incomingDebuff);
                break;
            case IStatsBuffEffect statsBuffEffect:
                _buffEffects.Remove(statsBuffEffect);
                _battleStats = statsBuffEffect.Remove(this);
                break;
            default:
                _tickEffects.Remove(effect);
                break;
        }
    }

    public bool CanSpendAction(ActionType actionType) => _actionBank.CanSpendAction(actionType);

    public bool TrySpendAction(ActionType actionType) => _actionBank.TrySpendAction(actionType);

    public void AddAction(ActionType actionType) => _actionBank.AddAction(actionType);

    public void ClearActions() => _actionBank.ClearActions();

    private void TickEffects()
    {
        for (var i = 0; i < _tickEffects.Count; i++)
        {
            _tickEffects[i].Tick(this);
        }

        for (var i = 0; i < _buffEffects.Count; i++)
        {
            _buffEffects[i].Tick(this);
        }

        for (var i = 0; i < _incomingTickEffects.Count; i++)
        {
            _incomingTickEffects[i].Tick(this);
        }
    }

    private void RefreshActions() => _actionBank.RefreshActions();
}

[Serializable]
public class ActionBank
{
    [ShowInInspector]
    private readonly List<ActionType> _availableActions = new();

    public IReadOnlyList<ActionType> AvailableActions => _availableActions;

    public bool CanSpendAction(ActionType actionType)
    {
        return actionType == ActionType.Free ||
               (actionType == ActionType.Difficult && _availableActions.Count == 2) ||
               _availableActions.Contains(actionType);
    }

    public bool TrySpendAction(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Difficult:
                if (_availableActions.Count < 2)
                    return false;
                _availableActions.Clear();
                return true;
            case ActionType.Free:
                return true;
            case ActionType.Movement:
            case ActionType.Ability:
            default:
                _availableActions.Remove(actionType);
                return true;
        }
    }

    public void AddAction(ActionType actionType) => _availableActions.Add(actionType);

    public void ClearActions() => _availableActions.Clear();

    public void RefreshActions()
    {
        ClearActions();
        _availableActions.Add(ActionType.Movement);
        _availableActions.Add(ActionType.Ability);
    }
}