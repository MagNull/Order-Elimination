using OrderElimination;
using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using VContainer;

namespace Assets.AbilitySystem.PrototypeHelpers
{
    public class BattleInitializer
    {
        private BattleMapDirector _battleMapDirector;
        private CharactersMediator _characterMediator;
        private BattleEntitiesFactory _entitiesFactory;
        private BattleEntitiesBank _entitiesBank;
        private IBattleMap _battleMap => _battleMapDirector.Map;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _battleMapDirector = objectResolver.Resolve<BattleMapDirector>();
            _characterMediator = objectResolver.Resolve<CharactersMediator>();
            _entitiesFactory = objectResolver.Resolve<BattleEntitiesFactory>();
            _entitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
        }

        public void InitiateBattle(BattleScenario scenario)
        {
            _battleMapDirector.InitializeMap();
            _entitiesBank.Clear();
            var gameAllies = CreateGameEntities(_characterMediator.GetPlayerCharactersInfo());
            var gameEnemies = CreateGameEntities(_characterMediator.GetEnemyCharactersInfo());
            var allies = _entitiesFactory.CreateBattleEntities(gameAllies, BattleSide.Player).ToArray();
            var enemies = _entitiesFactory.CreateBattleEntities(gameEnemies, BattleSide.Enemies).ToArray();
            foreach (var pos in scenario.MapObjects.Keys)
            {
                var obj = _entitiesFactory.CreateBattleObject(scenario.MapObjects[pos], BattleSide.Neutral);
                PlaceEntity(obj, pos);
            }
            PlaceEntities(allies, scenario.AlliesSpawnPositions);
            PlaceEntities(enemies, scenario.EnemySpawnPositions);
        }

        private void PlaceEntities(IEnumerable<CreatedEntity> entities, IEnumerable<Vector2Int> positions)
        {
            var entitiesArray = entities.ToArray();
            var positionsArray = positions.ToArray();
            if (entitiesArray.Length > positionsArray.Length)
                throw new InvalidOperationException();
            for (var i = 0; i < entitiesArray.Length; i++)
            {
                PlaceEntity(entitiesArray[i], positionsArray[i]);
            }
        }

        private void PlaceEntity(CreatedEntity entity, Vector2Int position)
        {
            _battleMap.PlaceEntity(entity.Model, position);
            entity.View.EntityPlacedOnMapCallback();
            _entitiesBank.AddEntity(entity.Model, entity.View);
        }

        private GameCharacter CreateGameEntity(IBattleEntityInfo entityInfo)
        {
            var activeAbilities = entityInfo.GetActiveAbilities().Select(a => AbilityFactory.CreateAbility(a)).ToArray();
            return new GameCharacter(entityInfo, activeAbilities, null);
        }

        private IEnumerable<GameCharacter> CreateGameEntities(IEnumerable<IBattleEntityInfo> entityInfos)
            => entityInfos.Select(gameEntity => CreateGameEntity(gameEntity));
    }
}
