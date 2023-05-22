using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
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
            var gameAllies = CreateGameEntities(_characterMediator.GetPlayerCharactersInfo()).ToArray();
            var gameEnemies = CreateGameEntities(_characterMediator.GetEnemyCharactersInfo()).ToArray();
            for (var i = 0; i < gameAllies.Length; i++)
            {
                var entity = gameAllies[i];
                var position = scenario.AlliesSpawnPositions[i];
                _entitiesFactory.CreateBattleCharacter(entity, BattleSide.Player, position);
            }
            for (var i = 0; i < gameEnemies.Length; i++)
            {
                var entity = gameEnemies[i];
                var position = scenario.EnemySpawnPositions[i];
                _entitiesFactory.CreateBattleCharacter(entity, BattleSide.Enemies, position);
            }
            foreach (var pos in scenario.MapObjects.Keys)
            {
                _entitiesFactory.CreateBattleStructure(scenario.MapObjects[pos], BattleSide.NoSide, pos);
            }
        }

        //TODO Extract GameCharacter creation outside battle
        private GameCharacter CreateGameEntity(IBattleCharacterData entityInfo)
        {
            var activeAbilities = entityInfo.GetActiveAbilities().Select(a => AbilityFactory.CreateActiveAbility(a));
            var passiveAbilities = entityInfo.GetPassiveAbilities().Select(a => AbilityFactory.CreatePassiveAbility(a));
            return new GameCharacter(entityInfo, activeAbilities, passiveAbilities);
        }

        private IEnumerable<GameCharacter> CreateGameEntities(IEnumerable<IBattleCharacterData> entityInfos)
            => entityInfos.Select(gameEntity => CreateGameEntity(gameEntity));
    }
}
