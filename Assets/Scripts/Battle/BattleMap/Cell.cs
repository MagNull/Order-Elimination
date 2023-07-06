using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.AbilitySystem;

public interface IReadOnlyCell
{
    public bool Contains(AbilitySystemActor entity);
    public IReadOnlyList<AbilitySystemActor> GetContainingEntities();
}

public class Cell : IReadOnlyCell
{
    private readonly HashSet<AbilitySystemActor> _containedEntitiesHash = new HashSet<AbilitySystemActor>();
    private readonly List<AbilitySystemActor> _containedEntities = new List<AbilitySystemActor>();

    public bool Contains(AbilitySystemActor entity) => _containedEntitiesHash.Contains(entity);

    public IReadOnlyList<AbilitySystemActor> GetContainingEntities() => _containedEntities;

    public bool AddEntity(AbilitySystemActor entity)
    {
        if (!Contains(entity))
        {
            _containedEntitiesHash.Add(entity);
            _containedEntities.Add(entity);
            return true;
        }
        return false;
    }

    public bool RemoveEntity(AbilitySystemActor entity)
    {
        if (Contains(entity))
        {
            _containedEntitiesHash.Remove(entity);
            _containedEntities.Remove(entity);
            return true;
        }
        return false;
    }
}