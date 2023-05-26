using Assets.AbilitySystem.PrototypeHelpers;
using CharacterAbility;
using OrderElimination.AbilitySystem;
using OrderElimination.Battle;
using OrderElimination.BM;
using OrderElimination.Domain;
using OrderElimination.Infrastructure;
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
        var icon = character.CharacterData.BattleIcon;
        var name = character.CharacterData.Name;

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        _entitiesBank.AddCharacterEntity(battleEntity, entityView, character.CharacterData);
        entityView.Initialize(battleEntity, icon, name);

        return new CreatedEntity(entityView, battleEntity);
    }

    public CreatedEntity CreateBattleStructure(IBattleStructureData structureData, BattleSide side, Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            throw new ArgumentOutOfRangeException("Position is not within map borders");

        var stats = new ReadOnlyBaseStats(structureData.MaxHealth, 0, 0, 0, 0, 0);
        var battleStats = new BattleStats(stats);
        var activeAbilities = structureData.GetPossesedAbilities().Select(a => AbilityFactory.CreateAbility(a)).ToArray();
        var battleEntity = new AbilitySystemActor(_battleContext, battleStats, EntityType.Structure, side, activeAbilities);

        var entityView = _objectResolver.Instantiate(_entityPrefab);
        var icon = structureData.BattleIcon;
        var name = structureData.Name;

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        _entitiesBank.AddStructureEntity(battleEntity, entityView, structureData);
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
