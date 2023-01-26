using System;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using CharacterAbility.BuffEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BM;
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
    public event Action Casted;
    public event Action<Cell, Cell> Moved;
    public event Action<BattleCharacter> Died;
    public event Action<ITickEffect> EffectAdded;
    public event Action<ITickEffect> EffectRemoved;

    [ShowInInspector]
    private readonly List<ITickEffect> _tickEffects;
    [ShowInInspector]
    private readonly List<IncomingBuff> _incomingTickEffects;
    [ShowInInspector]
    private readonly List<StatsBuffEffect> _buffEffects;
    [ShowInInspector]
    private readonly BattleObjectSide _side;
    [SerializeField]
    private BattleStats _battleStats;
    private IDamageCalculation _damageCalculation;

    [ShowInInspector]
    private readonly ActionBank _actionBank;

    public IReadOnlyList<ITickEffect> CurrentTickEffects => _tickEffects;
    public IReadOnlyList<IncomingBuff> IncomingTickEffects => _incomingTickEffects;
    public IReadOnlyList<StatsBuffEffect> CurrentBuffEffects => _buffEffects;

    public IReadOnlyList<ITickEffect> AllEffects =>
        _buffEffects;

    public BattleObjectSide Side => _side;
    public IBattleObjectView View { get; set; }

    public IReadOnlyList<ActionType> AvailableActions => _actionBank.AvailableActions;

    public IReadOnlyBattleStats Stats => _battleStats;

    public BattleCharacter(BattleObjectSide side, BattleStats battleStats, IDamageCalculation damageCalculation)
    {
        _damageCalculation = damageCalculation;
        _side = side;
        _battleStats = battleStats;
        _tickEffects = new List<ITickEffect>();
        _buffEffects = new List<StatsBuffEffect>();
        _incomingTickEffects = new List<IncomingBuff>();
        _actionBank = new ActionBank();
    }

    public void OnMoved(Cell from, Cell to) => Moved?.Invoke(from, to);

    public TakeDamageInfo TakeDamage(DamageInfo damageInfo)
    {
        if (_battleStats.Health <= 0)
            return new TakeDamageInfo();
        
        var damageTaken =
            _damageCalculation.CalculateDamage(damageInfo, _battleStats.Armor + _battleStats.AdditionalArmor,
                _battleStats.Evasion, _incomingTickEffects);
        var takeDamageInfo = new TakeDamageInfo
        {
            HealthDamage = damageTaken.healthDamage,
            ArmorDamage = damageTaken.armorDamage,
            CancelType = damageTaken.cancelType,
            Attacker = damageInfo.Attacker,
        };

        if (_battleStats.AdditionalArmor > 0)
        {
            var armorDamage = Mathf.Min(_battleStats.AdditionalArmor, damageTaken.armorDamage);
            _battleStats.AdditionalArmor -= armorDamage;
            takeDamageInfo.ArmorDamage -= armorDamage;
        }

        //TODO: Additional armor
        _battleStats.Armor -= damageTaken.armorDamage;
        _battleStats.Health -= damageTaken.healthDamage;
        
        Damaged?.Invoke(takeDamageInfo);

        if (_battleStats.Health > 0)
            return takeDamageInfo;
        _battleStats.Health = 0;
        Died?.Invoke(this);

        return takeDamageInfo;
    }

    //TODO: Strategy pattern in future if needed
    public void TakeRecover(int value, int accuracy, DamageHealTarget damageHealTarget)
    {
        Debug.Log("healed");
        // var isHeal = Random.Range(0, 100) < accuracy;
        // if (!isHeal)
        //     return;

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

        var takeDamageInfo = new TakeDamageInfo
        {
            HealthDamage = 0,
            ArmorDamage = 0,
            CancelType = 0,
            Attacker = new NullBattleObject(),
        };
        Damaged?.Invoke(takeDamageInfo);
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
        if (attacker is not BattleCharacter attackerCharacter)
            return accuracy;
        
        var info = new DamageInfo()
        {
            Attacker = attacker,
            Target = this,
        };
        foreach (var effect in attackerCharacter.CurrentTickEffects.Where(ef => ef is OutcomingBuff))
        {
            info.Accuracy = accuracy;
            accuracy = ((OutcomingBuff) effect).GetModifiedInfo(info).Accuracy;
        }

        accuracy = Mathf.Clamp(accuracy, 0, 100);
        return accuracy;
    }

    public void OnTurnStart()
    {
        if (!_tickEffects.Any(ef => ef is StunBuff))
        {
            Debug.Log("Check");
            RefreshActions();
        }
        TickEffects();
    }

    public void AddTickEffect(ITickEffect effect)
    {
        switch (effect)
        {
            case IncomingBuff incomingDebuff:
                _incomingTickEffects.Add(incomingDebuff);
                break;
            case StatsBuffEffect statsBuffEffect:
                _buffEffects.Add(statsBuffEffect);
                _battleStats = statsBuffEffect.Apply(this);
                break;
            case StunBuff stun:
                stun.Apply(this);
                _tickEffects.Add(stun);
                break;
            default:
                _tickEffects.Add(effect);
                break;
        }

        EffectAdded?.Invoke(effect);
    }

    public virtual UniTask PlayTurn()
    {
        return UniTask.CompletedTask;
    }

    public void RemoveTickEffect(ITickEffect effect)
    {
        switch (effect)
        {
            case IncomingBuff incomingDebuff:
                _incomingTickEffects.Remove(incomingDebuff);
                break;
            case StatsBuffEffect statsBuffEffect:
                _buffEffects.Remove(statsBuffEffect);
                _battleStats = statsBuffEffect.Remove(this);
                break;
            default:
                _tickEffects.Remove(effect);
                break;
        }

        EffectRemoved?.Invoke(effect);
    }

    public bool CanSpendAction(ActionType actionType) => _actionBank.CanSpendAction(actionType);

    public bool TrySpendAction(ActionType actionType)
    {
        var actionPerformed = _actionBank.TrySpendAction(actionType);
        return actionPerformed;
    }

    public void AddAction(ActionType actionType) => _actionBank.AddAction(actionType);

    public void ClearActions() => _actionBank.ClearActions();

    public void OnCasted(ActionType actionType)
    {
        if (actionType == ActionType.Movement)
            return;
        Casted?.Invoke();
    }

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
                if (_availableActions.All(action => action != actionType))
                    return false;
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