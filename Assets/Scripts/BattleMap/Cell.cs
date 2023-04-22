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
    public IReadOnlyList<IAbilitySystemActor> GetContainingEntities();
}

public class Cell : IReadOnlyCell
{
    #region ToRemove
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
    #endregion

    private readonly HashSet<IAbilitySystemActor> _containedEntitiesHash = new HashSet<IAbilitySystemActor>();
    private readonly List<IAbilitySystemActor> _containedEntities = new List<IAbilitySystemActor>();

    public bool Contains(IAbilitySystemActor entity) => _containedEntitiesHash.Contains(entity);

    public bool AddEntity(IAbilitySystemActor entity)
    {
        if (!Contains(entity))
        {
            _containedEntitiesHash.Add(entity);
            _containedEntities.Add(entity);
            return true;
        }
        return false;
    }

    public bool RemoveEntity(IAbilitySystemActor entity)
    {
        if (Contains(entity))
        {
            _containedEntitiesHash.Remove(entity);
            _containedEntities.Remove(entity);
            return true;
        }
        return false;
    }

    public IReadOnlyList<IAbilitySystemActor> GetContainingEntities() => _containedEntities;
}