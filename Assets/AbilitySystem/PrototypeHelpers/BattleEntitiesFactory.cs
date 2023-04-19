using CharacterAbility;
using OrderElimination.AbilitySystem;
using OrderElimination.Battle;
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
    private IBattleMap _battleMap;
    private IObjectResolver _objectResolver;

    [Inject]
    public void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _battleMap = objectResolver.Resolve<IBattleMap>();
    }

    public CreatedEntity CreateBattleEntity(GameEntity gameEntity, BattleSide side)
    {
        var battleEntity = new IAbilitySystemActor(_battleMap, gameEntity.BattleStats, gameEntity.EntityType, side, gameEntity.PosessedActiveAbilities.ToArray());

        var entityView = _objectResolver.Instantiate(_entityPrefab);
        var icon = gameEntity.EntityData.BattleIcon;
        var name = gameEntity.EntityData.Name;
        entityView.Initialize(battleEntity, icon, name);

        return new CreatedEntity(entityView, battleEntity);
    }

    public IEnumerable<CreatedEntity> CreateBattleEntities(IEnumerable<GameEntity> entities, BattleSide side)
        => entities.Select(gameEntity => CreateBattleEntity(gameEntity, side));

    //TODO: extract ouside battle
    public GameEntity CreateGameEntity(IBattleEntityInfo entityInfo)
    {
        return new GameEntity(entityInfo);
    }

    public IEnumerable<GameEntity> CreateGameEntities(IEnumerable<IBattleEntityInfo> entityInfos)
        => entityInfos.Select(gameEntity => CreateGameEntity(gameEntity));
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
