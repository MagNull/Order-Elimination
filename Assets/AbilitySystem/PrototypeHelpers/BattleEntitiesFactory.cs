using Assets.AbilitySystem.PrototypeHelpers;
using CharacterAbility;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.OuterComponents;
using OrderElimination.Battle;
using OrderElimination.BM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleEntitiesFactory : MonoBehaviour
{
    [SerializeField]
    private BattleEntityView _entityPrefab;
    [SerializeField]
    private Transform _entitiesParent;
    private IBattleContext _battleContext;
    private BattleEntitiesBank _entitiesBank;
    private IObjectResolver _objectResolver;

    [Inject]
    public void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _battleContext = objectResolver.Resolve<IBattleContext>();
        _entitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
    }

    public CreatedEntity CreateBattleCharacter(GameCharacter character, BattleSide side, Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            throw new ArgumentOutOfRangeException("Position is not within map borders");

        var battleEntity = new AbilitySystemActor(_battleContext, character.BattleStats, EntityType.Character, side, character.PosessedActiveAbilities.ToArray());

        var entityView = _objectResolver.Instantiate(_entityPrefab, _entitiesParent);
        var icon = character.EntityData.BattleIcon;
        var name = character.EntityData.Name;

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        _entitiesBank.AddEntity(battleEntity, entityView);
        entityView.Initialize(battleEntity, icon, name);

        return new CreatedEntity(entityView, battleEntity);
    }

    public CreatedEntity CreateBattleObject(EnvironmentInfo objectInfo, BattleSide side, Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            throw new ArgumentOutOfRangeException("Position is not within map borders");

        var stats = new ReadOnlyBaseStats(objectInfo.MaxHealth, 0, 0, 0, 0, 0);
        var battleStats = new BattleStats(stats);
        var activeAbilities = objectInfo.GetActiveAbilities().Select(a => AbilityFactory.CreateAbility(a)).ToArray();
        var battleEntity = new AbilitySystemActor(_battleContext, battleStats, EntityType.MapObject, side, activeAbilities);

        var entityView = _objectResolver.Instantiate(_entityPrefab);
        var icon = objectInfo.BattleIcon;
        var name = objectInfo.Name;

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        _entitiesBank.AddEntity(battleEntity, entityView);
        entityView.Initialize(battleEntity, icon, name);

        return new CreatedEntity(entityView, battleEntity);
    }
}

public readonly struct CreatedEntity
{
    public readonly BattleEntityView View;
    public readonly AbilitySystemActor Model;

    public CreatedEntity(BattleEntityView view, AbilitySystemActor model)
    {
        View = view;
        Model = model;
    }
}
