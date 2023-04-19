using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Battle;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

[Serializable]
public class CharacterArrangeDirector
{
    #region NewAbilitySystem
    private readonly BattleEntitiesFactory _entitiesFactory;
    private readonly Dictionary<IAbilitySystemActor, BattleEntityView> _viewsByEntities;
    private readonly Dictionary<BattleEntityView, IAbilitySystemActor> _entitiesByViews;

    public BattleEntityView GetViewByEntity(IAbilitySystemActor entity) => _viewsByEntities[entity];
    public IAbilitySystemActor GetEntityByView(BattleEntityView view) => _entitiesByViews[view];

    //public IAbilitySystemActor[] ArrangeEntities(IEnumerable<Vector2Int> alliesPositions,
    //    IEnumerable<Vector2Int> enemyPositions)
    //{
    //    var characters = new List<IAbilitySystemActor>();
    //    var gameAllies = _entitiesFactory.CreateGameEntities(_charactersMediator.GetPlayerCharactersInfo());
    //    var gameEnemies = _entitiesFactory.CreateGameEntities(_charactersMediator.GetEnemyCharactersInfo());
    //    var battleAllies = _entitiesFactory.CreateBattleEntities(gameAllies, BattleSide.Player).ToArray();
    //    var battleEnemies = _entitiesFactory.CreateBattleEntities(gameEnemies, BattleSide.Enemies).ToArray();
    //    for (var i = 0; i < battleAllies.Length; i++)
    //    {
    //        _arrangementMap.PlaceEntity(battleAllies[i].Model, alliesPositions[i]);
    //        battleAllies[i].View.EntityPlacedOnMapCallback();

    //    }
    //    for (var i = 0; i < battleEnemies.Length; i++)
    //    {
    //        _arrangementMap.PlaceEntity(battleEnemies[i].Model, enemyPositions[i]);
    //        battleEnemies[i].View.EntityPlacedOnMapCallback();
    //    }
    //    foreach (var entity in battleAllies.Concat(battleEnemies))
    //    {
    //        characters.Add(entity.Model);
    //        _entitiesByViews.Add(entity.View, entity.Model);
    //        _viewsByEntities.Add(entity.Model, entity.View);
    //    }
    //    return characters.ToArray();
    //}
    public IAbilitySystemActor[] ArrangeEntities(IEnumerable<Vector2Int> positions, BattleSide side)
    {
        var positionsList = positions.ToList();
        var characters = new List<IAbilitySystemActor>();
        var characterInfos =
            side == BattleSide.Player 
            ? _charactersMediator.GetPlayerCharactersInfo()
            : side == BattleSide.Enemies
            ? _charactersMediator.GetEnemyCharactersInfo()
            : throw new NotImplementedException();
        var gameEntities = _entitiesFactory.CreateGameEntities(characterInfos);
        var battleEntities = _entitiesFactory.CreateBattleEntities(gameEntities, BattleSide.Player).ToArray();
        for (var i = 0; i < battleEntities.Length; i++)
        {
            var entity = battleEntities[i];
            _arrangementMap.PlaceEntity(entity.Model, positionsList[i]);
            entity.View.EntityPlacedOnMapCallback();
            characters.Add(entity.Model);
            _entitiesByViews.Add(entity.View, entity.Model);
            _viewsByEntities.Add(entity.Model, entity.View);
        }
        return characters.ToArray();
    }
    #endregion

    private readonly BattleCharacterFactory _characterFactory;
    private readonly CharactersBank _charactersBank;

    private BattleMap _arrangementMap;
    private CharactersMediator _charactersMediator;

    [Inject]
    private CharacterArrangeDirector(CharactersMediator charactersMediator, BattleCharacterFactory characterFactory,
        CharactersBank charactersBank, BattleEntitiesFactory entitiesFactory)
    {
        _charactersMediator = charactersMediator;
        _characterFactory = characterFactory;
        _charactersBank = charactersBank;
        _entitiesFactory = entitiesFactory;
        _entitiesByViews = new Dictionary<BattleEntityView, IAbilitySystemActor>();
        _viewsByEntities = new Dictionary<IAbilitySystemActor, BattleEntityView>();
    }

    public void SetArrangementMap(BattleMap map) => _arrangementMap = map;

    public List<BattleCharacter> Arrange(List<Vector2Int> unitPositions,
        List<Vector2Int> enemyPositions)
    {
        var characters = new List<BattleCharacter>();

        List<BattleCharacter> playerSquad =
            _characterFactory.CreatePlayerSquad(_charactersMediator.GetPlayerCharactersInfo());
        for (int i = 0; i < playerSquad.Count; i++)
        {
            characters.Add(playerSquad[i]);
            playerSquad[i].Died += OnCharacterDied;
            _arrangementMap.SpawnObject(playerSquad[i], unitPositions[i].x, unitPositions[i].y);
        }

        List<BattleCharacter> enemySquad = _characterFactory.CreateEnemySquad(_charactersMediator.GetEnemyCharactersInfo());
        for (int i = 0; i < enemySquad.Count; i++)
        {
            characters.Add(enemySquad[i]);
            enemySquad[i].Died += OnCharacterDied;
            _arrangementMap.SpawnObject(enemySquad[i], enemyPositions[i].x, enemyPositions[i].y);
        }
        
        _charactersBank.AddCharactersRange(characters);

        return characters;
    }

    private void OnCharacterDied(BattleCharacter battleObject)
    {
        _arrangementMap.DestroyObject(battleObject);
        _charactersBank.RemoveCharacter(battleObject);
        battleObject.Died -= OnCharacterDied;
    }
}