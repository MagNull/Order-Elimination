using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.BM;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using OrderElimination.AbilitySystem;

public interface IReadOnlyCell
{
    public IReadOnlyList<IBattleObject> Objects { get; }
    public bool Contains(Predicate<IBattleObject> predicate, out IBattleObject result);
    public IEnumerable<IAbilitySystemActor> GetContainingEntities();
}

public class Cell : IReadOnlyCell
{
    private readonly List<IBattleObject> _objects;

    public IReadOnlyList<IBattleObject> Objects => _objects;

    public Cell()
    {
        _objects = new List<IBattleObject> {new NullBattleObject()};
    }

    public void AddObject(IBattleObject obj)
    {
        if (_objects.Contains(obj))
        {
            Debug.LogWarning("Try add existing object to cell");
            return;
        }

        _objects.Add(obj);
    }

    public bool Contains(Predicate<IBattleObject> predicate, out IBattleObject result)
    {
        result = _objects.Find(predicate);
        return result != null;
    }

    public void RemoveObject(IBattleObject obj)
    {
        if (!_objects.Contains(obj))
        {
            Debug.LogWarning("Try remove wrong object from cell");
            return;
        }

        _objects.Remove(obj);
    }

    public IEnumerable<IAbilitySystemActor> GetContainingEntities()
    {
        return (IEnumerable<IAbilitySystemActor>)_objects;
    }
}