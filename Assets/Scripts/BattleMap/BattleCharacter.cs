using System;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

[Serializable]
public class BattleCharacter : IBattleObject
{
    public event Action<int> Damaged;
    
    [SerializeField]
    private float _health;

    private List<ITickEffect> _activeEffects;
    private readonly BattleObjectSide _side;
    private readonly BattleStats _battleStats;
    private BattleCharacterView _view;

    public float Health => _health;
    public BattleObjectSide Side => _side;
    public BattleStats Stats => _battleStats;

    public BattleCharacter(BattleObjectSide side, BattleStats battleStats)
    {
        _side = side;
        _battleStats = battleStats;
        _health = _battleStats.Health;
        _activeEffects = new List<ITickEffect>();
    }
    
    public void SetView(BattleCharacterView view) => _view = view;

    public void TickEffects()
    {
        foreach (ITickEffect effect in _activeEffects)
        {
            effect.Tick();
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            Debug.Log("Character is dead");
        }
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
        // Пустой пока
    }
}
