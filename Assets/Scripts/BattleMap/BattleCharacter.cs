using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour, IBattleObject
{
    private List<ITickEffect> _activeEffects;
    private CharacterSide _side;
    private BattleStats _battleStats;

    public void OnEnable()
    {
        BattleCharacterView.BattleCharacterViewClicked += OnClicked;
    }

    public void OnDisable()
    {
        BattleCharacterView.BattleCharacterViewClicked -= OnClicked;
    }

    public BattleStats GetStats() => _battleStats;
    public CharacterSide GetSide() => _side;

    public BattleCharacterView GetView()
    {
        // Реализовать
        throw new System.NotImplementedException();
    }

    public void TickEffects()
    {
        foreach (ITickEffect effect in _activeEffects)
        {
            effect.Tick();
        }
    }

    public void TakeDamage(int damage)
    {
        _battleStats.ChangeHp(-damage);
    }

    public void AddTickEffect(ITickEffect effect)
    {
        _activeEffects.Add(effect);
    }

    public void RemoveTickEffect(ITickEffect effect)
    {
        _activeEffects.Remove(effect);
    }

    public void OnClicked()
    {
        // Пустой пока

        // Тест работоспособности ивентов
        Debug.Log("Произошло событие класса: BattleCharacter");
    }

    public void OnTurnStart()
    {
        // Пустой пока
    }
}
