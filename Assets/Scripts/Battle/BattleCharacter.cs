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
public class BattleCharacter : IAbilityCaster //TODO: Add IAbilityCaster like interface for ability
{
    public event Action<int, int, DamageCancelType> Damaged;
    public event Action<BattleCharacter> Died;

    private readonly List<ITickEffect> _tickEffects;
    private readonly List<IncomingDebuff> _incomingTickEffects;
    private readonly List<IStatsBuffEffect> _buffEffects;
    [ShowInInspector]
    private readonly BattleObjectSide _side;
    private BattleCharacterView _view;
    [SerializeField]
    private BattleStats _battleStats;
    private IDamageCalculation _damageCalculation;

    [ShowInInspector]
    private readonly List<ActionType> _availableActions;

    public BattleObjectSide Side => _side;

    public IReadOnlyList<ActionType> AvailableActions => _availableActions;

    public IReadOnlyBattleStats Stats => _battleStats;

    public BattleCharacter(BattleObjectSide side, BattleStats battleStats, IDamageCalculation damageCalculation)
    {
        _damageCalculation = damageCalculation;
        _side = side;
        _battleStats = battleStats;
        _tickEffects = new List<ITickEffect>();
        _buffEffects = new List<IStatsBuffEffect>();
        _incomingTickEffects = new List<IncomingDebuff>();
        _availableActions = new List<ActionType>();
    }

    public void SetView(BattleCharacterView view) => _view = view;

    public GameObject GetView() => _view.gameObject;

    public void TakeDamage(int damage, int accuracy, DamageHealType damageHealType)
    {
        var damageTaken =
            _damageCalculation.CalculateDamage(damage, _battleStats.Armor, accuracy, _battleStats.Evasion,
                damageHealType, _incomingTickEffects);
        Damaged?.Invoke(damageTaken.armorDamage, damageTaken.healtDamage, damageTaken.damageCancelType);
        _battleStats.Armor -= damageTaken.armorDamage;
        _battleStats.Health -= damageTaken.healtDamage;
        if (_battleStats.Health <= 0)
        {
            _battleStats.Health = 0;
            Died?.Invoke(this);
        }
    }

    //TODO: Strategy pattern in future if needed

    public void TakeRecover(int value, int accuracy, DamageHealType damageHealType)
    {
        bool isHeal = Random.Range(0, 100) < accuracy;
        if (!isHeal)
            return;

        switch (damageHealType)
        {
            case DamageHealType.OnlyHealth:
                _battleStats.Health += value;
                break;
            case DamageHealType.OnlyArmor:
                _battleStats.Armor += value;
                break;
            case DamageHealType.Normal:
            default:
                _battleStats.Health += value;
                break;
        }
    }
    
    public void ClearOverEffects()
    {
        _tickEffects.Clear();
    }

    public void OnTurnStart()
    {
        TickEffects();
        RefreshActions();
    }

    private void TickEffects()
    {
        for (var i = 0; i < _tickEffects.Count; i++)
        {
            _tickEffects[i].Tick();
        }
        for (var i = 0; i < _buffEffects.Count; i++)
        {
            _buffEffects[i].Tick();
        }
    }
    
    public void AddTickEffect(ITickEffect effect)
    {
        switch (effect)
        {
            case IncomingDebuff incomingDebuff:
                _incomingTickEffects.Add(incomingDebuff);
                break;
            case IStatsBuffEffect statsBuffEffect:
                _buffEffects.Add(statsBuffEffect);
                _battleStats = statsBuffEffect.Apply();
                break;
            default:
                _tickEffects.Add(effect);
                break;
        }
    }

    public virtual void PlayTurn()
    { }

    public void RemoveTickEffect(ITickEffect effect)
    {
        switch (effect)
        {
            case IncomingDebuff incomingDebuff:
                _incomingTickEffects.Remove(incomingDebuff);
                break;
            case IStatsBuffEffect statsBuffEffect:
                _buffEffects.Remove(statsBuffEffect);
                _battleStats = statsBuffEffect.Remove();
                break;
            default:
                _tickEffects.Remove(effect);
                break;
        }
    }

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

    public void ClearBuffEffects()
    {
        foreach (var buffEffect in _buffEffects)
        {
            _battleStats = buffEffect.Remove();
        }
        _buffEffects.Clear();
    }

    public void AddAction(ActionType actionType) => _availableActions.Add(actionType);

    private void RefreshActions()
    {
        _availableActions.Clear();
        _availableActions.Add(ActionType.Movement);
        _availableActions.Add(ActionType.Ability);
    }
}