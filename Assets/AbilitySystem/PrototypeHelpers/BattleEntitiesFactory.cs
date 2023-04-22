using CharacterAbility;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.OuterComponents;
using OrderElimination.Battle;
using OrderElimination.BM;
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
    private IBattleMap _battleMap;
    private IObjectResolver _objectResolver;

    [Inject]
    public void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _battleMap = objectResolver.Resolve<IBattleMap>();
    }

    public CreatedEntity CreateBattleCharacter(GameCharacter character, BattleSide side)
    {
        var battleEntity = new IAbilitySystemActor(_battleMap, character.BattleStats, EntityType.Character, side, character.PosessedActiveAbilities.ToArray());

        var entityView = _objectResolver.Instantiate(_entityPrefab, _entitiesParent);
        var icon = character.EntityData.BattleIcon;
        var name = character.EntityData.Name;
        entityView.Initialize(battleEntity, icon, name);

        return new CreatedEntity(entityView, battleEntity);
    }

    public CreatedEntity CreateBattleObject(EnvironmentInfo objectInfo, BattleSide side)
    {
        var stats = new ReadOnlyBaseStats(objectInfo.MaxHealth, 0, 0, 0, 0, 0);
        var battleStats = new BattleStats(stats);
        var activeAbilities = objectInfo.GetActiveAbilities().Select(a => AbilityFactory.CreateAbility(a)).ToArray();
        var battleEntity = new IAbilitySystemActor(_battleMap, battleStats, EntityType.MapObject, side, activeAbilities);

        var entityView = _objectResolver.Instantiate(_entityPrefab);
        var icon = objectInfo.BattleIcon;
        var name = objectInfo.Name;
        entityView.Initialize(battleEntity, icon, name);

        return new CreatedEntity(entityView, battleEntity);
    }

    public IEnumerable<CreatedEntity> CreateBattleEntities(IEnumerable<GameCharacter> entities, BattleSide side)
        => entities.Select(gameEntity => CreateBattleCharacter(gameEntity, side));
    public IEnumerable<CreatedEntity> CreateBattleEntities(IEnumerable<EnvironmentInfo> entities, BattleSide side)
        => entities.Select(gameEntity => CreateBattleObject(gameEntity, side));
}

public readonly struct CreatedEntity
{
    public readonly BattleEntityView View;
    public readonly IAbilitySystemActor Model;

    public CreatedEntity(BattleEntityView view, IAbilitySystemActor model)
    {
        View = view;
        Model = model;
    }
}
