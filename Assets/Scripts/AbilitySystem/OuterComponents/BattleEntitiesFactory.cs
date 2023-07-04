using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MetaGame;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using VContainer;
using VContainer.Unity;

public class BattleEntitiesFactory : MonoBehaviour
{
    [SerializeField]
    private BattleEntityView _characterPrefab;

    [SerializeField]
    private BattleEntityView _structurePrefab;

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
            Logging.LogException(new ArgumentOutOfRangeException("Position is not within map borders"));

        var battleEntity = new AbilitySystemActor(
            _battleContext,
            new BattleStats(
                character.CharacterStats.MaxHealth,
                character.CharacterStats.MaxArmor,
                character.CharacterStats.AttackDamage,
                character.CharacterStats.Accuracy,
                character.CharacterStats.Evasion,
                character.CharacterStats.MaxMovementDistance),
            EntityType.Character,
            side,
            new EntityObstacleSetup());

        foreach (var abilityData in character.ActiveAbilities)
        {
            battleEntity.GrantActiveAbility(new ActiveAbilityRunner(abilityData, AbilityProvider.Self));
        }

        foreach (var abilityData in character.PassiveAbilities)
        {
            battleEntity.GrantPassiveAbility(new PassiveAbilityRunner(abilityData, AbilityProvider.Self));
        }

        foreach (var item in character.Inventory.GetItems())
            item.OnTook(battleEntity);

        var entityView = _objectResolver.Instantiate(_characterPrefab, _charactersParent);
        entityView.Initialize(battleEntity, character.CharacterData.Name, character.CharacterData.BattleIcon);

        _entitiesBank.AddCharacterEntity(battleEntity, entityView, character);

        _battleContext.BattleMap.PlaceEntity(battleEntity, position);
        battleEntity.PassiveAbilities.ForEach(a => a.Activate(_battleContext, battleEntity));

        return new CreatedBattleEntity(entityView, battleEntity);
    }

    public CreatedBattleEntity CreateBattleStructure(IBattleStructureTemplate structureData, BattleSide side,
        Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            Logging.LogException(new ArgumentOutOfRangeException("Position is not within map borders"));

        var battleStats = new BattleStats(structureData.MaxHealth, 0, 0, 0, 0, 0);
        var passiveAbilities = structureData.GetPossesedAbilities().Select(a => AbilityFactory.CreatePassiveAbility(a))
            .ToArray();
        var battleEntity = new AbilitySystemActor(
            _battleContext,
            battleStats,
            EntityType.Structure,
            side,
            structureData.ObstacleSetup);
        foreach (var abilityData in passiveAbilities)
        {
            battleEntity.GrantPassiveAbility(new PassiveAbilityRunner(abilityData, AbilityProvider.Self));
        }

        var entityView = _objectResolver.Instantiate(_structurePrefab, _structuresParent);
        entityView.Initialize(battleEntity, structureData.Name, structureData.BattleIcon, structureData.VisualModel);

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
        _entitiesBank.RemoveEntity(entity); //Called automatically on event Disposed
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