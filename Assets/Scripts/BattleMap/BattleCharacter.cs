using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour, IBattleObject
{
    private List<ITickEffect> _activeEffects;
    private CharacterSide _side;

    public void TickEffects()
    {
        foreach (ITickEffect effect in _activeEffects)
        {
            effect.Tick();
        }
    }

    public void TakeDamage()
    {

    }

    public BattleStats GetStats()
    {
        throw new System.NotImplementedException();
    }

    public CharacterSide GetSide() => _side;

    public void OnClicked()
    {

    }

    public void AddTickEffect(ITickEffect effect)
    {
        _activeEffects.Add(effect);
    }

    public void RemoveTickEffect(ITickEffect effect)
    {
        _activeEffects.Remove(effect);
    }

    public BattleCharacterView GetView()
    {
        throw new System.NotImplementedException();
    }

    public void OnTurnStart()
    {
        throw new System.NotImplementedException();
    }
}
