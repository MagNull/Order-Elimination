using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.Battle;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ActionType
{
    Movement,
    Ability,
    Free
}

[Serializable]
public class BattleCharacter : IAbilityCaster //TODO: Add IAbilityCaster like interface for ability
{
    public event Action<int> Damaged;

    private List<ITickEffect> _activeEffects;
    [ShowInInspector]
    private readonly BattleObjectSide _side;
    private BattleCharacterView _view;
    [SerializeField]
    private BattleStats _battleStats;
    private BattleStats _startStats;
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
        _startStats = battleStats;
        _activeEffects = new List<ITickEffect>();
        _availableActions = new List<ActionType>();
        BattleSimulation.RoundStarted += OnTurnStart;
    }

    public void SetView(BattleCharacterView view) => _view = view;

    public void TakeDamage(int damage, int accuracy)
    {
        var damageTaken =
            _damageCalculation.CalculateDamage(damage, _battleStats.Armor, accuracy, _battleStats.Evasion);
        if(damageTaken.healtDamage > 0 || damageTaken.armorDamage > 0)
            Debug.Log(GetView().name + " take damage");
        _battleStats.Armor -= damageTaken.armorDamage;
        _battleStats.Health -= damageTaken.healtDamage;
        if (_battleStats.Health <= 0)
        {
            _battleStats.Health = 0;
            Debug.Log("Character is dead");
        }
    }

    //TODO: Strategy pattern in future if needed

    public void TakeHeal(int heal, int accuracy)
    {
        bool isHeal = Random.Range(0, 100) < accuracy;
        if (!isHeal)
            return;

        _battleStats.Health = Mathf.Clamp(_battleStats.Health + heal, 0, _startStats.Health);
        Debug.Log(GetView().name + " take heal");
    }

    public void AddTickEffect(ITickEffect effect)
    {
        _activeEffects.Add(effect);
    }

    public void RemoveTickEffect(ITickEffect effect)
    {
        _activeEffects.Remove(effect);
    }

    public GameObject GetView() => _view.gameObject;

    public void OnTurnStart()
    {
        TickEffects();
        RefreshActions();
    }

    private void TickEffects()
    {
        for (var i = 0; i < _activeEffects.Count; i++)
        {
            _activeEffects[i].Tick();
        }
    }

    private void RefreshActions()
    {
        _availableActions.Clear();
        _availableActions.Add(ActionType.Movement);
        _availableActions.Add(ActionType.Ability);
    }

    public bool TrySpendAction(ActionType actionType)
    {
        if (!_availableActions.Contains(actionType)) 
            return false;
        _availableActions.Remove(actionType);
        return true;
    }

    public void AddAction(ActionType actionType) => _availableActions.Add(actionType);
}