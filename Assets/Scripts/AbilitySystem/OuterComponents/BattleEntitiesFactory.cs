using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleEntitiesFactory : SerializedMonoBehaviour
{
    [SerializeField]
    private BattleEntityView _characterPrefab;

    [SerializeField]
    private BattleEntityView _structurePrefab;

    [SerializeField]
    private Transform _charactersParent;

    [SerializeField]
    private Transform _structuresParent;

    [ShowInInspector, OdinSerialize]
    private IBattleObstacleSetup _charactersObstacleSetup;

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

    [Obsolete("Use " + nameof(EntitySpawner) + " instead.")]
    public CreatedBattleEntity CreateBattleCharacter(GameCharacter character, BattleSide side, Vector2Int position)
    {
        if (!_battleContext.BattleMap.CellRangeBorders.Contains(position))
            Logging.LogException(new ArgumentOutOfRangeException("Position is not within map borders"));

        var battleStats = new BattleStats(
                character.CharacterStats.MaxHealth,
                character.CharacterStats.MaxArmor,
                character.CharacterStats.Attack,
                character.CharacterStats.Accuracy,
                character.CharacterStats.Evasion,
                character.CharacterStats.MaxMovementDistance);
        battleStats.Health = character.CurrentHealth;
        var battleEntity = new AbilitySystemActor(
            _battleContext, battleStats, EntityType.Character, side, _charactersObstacleSetup);

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
        //battleEntity.PassiveAbilities.ForEach(a => a.Activate(_battleContext, battleEntity));

        return new CreatedBattleEntity(entityView, battleEntity);
    }

    [Obsolete("Use " + nameof(EntitySpawner) + " instead.")]
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
        //battleEntity.PassiveAbilities.ForEach(a => a.Activate(_battleContext, battleEntity));

        return new CreatedBattleEntity(entityView, battleEntity);
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