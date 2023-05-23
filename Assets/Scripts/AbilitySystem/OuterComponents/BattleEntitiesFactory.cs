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
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

public class BattleEntitiesFactory : MonoBehaviour
{
    [SerializeField]
    private BattleEntityView _entityPrefab;
    [SerializeField]
    private Transform _charactersParent;
    [SerializeField]
    private Transform _structuresParent;
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

    public CreatedBattleEntity CreateBattleCharacter(GameCharacter character, BattleSide side, Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            throw new ArgumentOutOfRangeException("Position is not within map borders");

        var battleEntity = new AbilitySystemActor(
            _battleContext, character.BattleStats, 
            EntityType.Character, 
            side, 
            character.PosessedActiveAbilities.ToArray(),
            character.PosessedPassiveAbilities.ToArray());

        var entityView = _objectResolver.Instantiate(_entityPrefab, _charactersParent);
        entityView.Initialize(battleEntity, character.CharacterData.BattleIcon, character.CharacterData.Name);

        _entitiesBank.AddCharacterEntity(battleEntity, entityView, character.CharacterData);

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        battleEntity.PassiveAbilities.ForEach(a => a.Activate(_battleContext, battleEntity));//

        return new CreatedBattleEntity(entityView, battleEntity);
    }

    public CreatedBattleEntity CreateBattleStructure(IBattleStructureData structureData, BattleSide side, Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            throw new ArgumentOutOfRangeException("Position is not within map borders");

        var stats = new ReadOnlyBaseStats(structureData.MaxHealth, 0, 0, 0, 0, 0);
        var battleStats = new BattleStats(stats);
        var passiveAbilities = structureData.GetPossesedAbilities().Select(a => AbilityFactory.CreatePassiveAbility(a)).ToArray();
        var battleEntity = new AbilitySystemActor(
            _battleContext, 
            battleStats, 
            EntityType.Structure, 
            side,
            new ActiveAbilityData[0],
            passiveAbilities);

        var entityView = _objectResolver.Instantiate(_entityPrefab, _structuresParent);
        entityView.Initialize(battleEntity, structureData.BattleIcon, structureData.Name);

        _entitiesBank.AddStructureEntity(battleEntity, entityView, structureData);

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        battleEntity.PassiveAbilities.ForEach(a => a.Activate(_battleContext, battleEntity));

        return new CreatedBattleEntity(entityView, battleEntity);
    }

    //TODO Move to entity.Dispose()
    public void DisposeEntity(AbilitySystemActor entity)
    {
        foreach (var activedPassiveRunner in entity.PassiveAbilities.Where(runner => runner.IsActive))
        {
            activedPassiveRunner.Deactivate();
        }
        //remove ALL effects //Automatically calls effect.Deactivate() on event entity.Disposed
        //? entity.Dispose(); event Disposed; ...
        _battleContext.BattleMap.RemoveEntity(entity);
        _entitiesBank.RemoveEntity(entity);//Called automatically on event Disposed
        //destroy EntityView //Called automatically on event Disposed
    }
}

public readonly struct CreatedBattleEntity
{
    public readonly BattleEntityView View;
    public readonly AbilitySystemActor Model;

    public CreatedBattleEntity(BattleEntityView view, AbilitySystemActor model)
    {
        View = view;
        Model = model;
    }
}
