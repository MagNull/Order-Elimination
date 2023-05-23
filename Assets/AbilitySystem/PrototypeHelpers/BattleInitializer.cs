﻿using OrderElimination;
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
        private GameCharactersFactory _gameCharactersFactory;
        private BattleEntitiesBank _entitiesBank;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _battleMapDirector = objectResolver.Resolve<BattleMapDirector>();
            _characterMediator = objectResolver.Resolve<CharactersMediator>();
            _entitiesFactory = objectResolver.Resolve<BattleEntitiesFactory>();
            _entitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
            _gameCharactersFactory = objectResolver.Resolve<GameCharactersFactory>();
        }

        public void InitiateBattle()
        {
            _battleMapDirector.InitializeMap();
            _entitiesBank.Clear();
        }

        public void StartScenario(BattleScenario scenario)
        {
            var gameAllies 
                = _gameCharactersFactory.CreateGameEntities(_characterMediator.GetPlayerCharactersInfo()).ToArray();
            var gameEnemies 
                = _gameCharactersFactory.CreateGameEntities(_characterMediator.GetEnemyCharactersInfo()).ToArray();
            var allySpawns = scenario.GetAlliesSpawnPositions();
            var enemySpawns = scenario.GetEnemySpawnPositions();
            for (var i = 0; i < gameAllies.Length; i++)
            {
                var entity = gameAllies[i];
                var position = allySpawns[i];
                _entitiesFactory.CreateBattleCharacter(entity, BattleSide.Player, position);
            }
            for (var i = 0; i < gameEnemies.Length; i++)
            {
                var entity = gameEnemies[i];
                var position = enemySpawns[i];
                _entitiesFactory.CreateBattleCharacter(entity, BattleSide.Enemies, position);
            }
            foreach (var pos in scenario.StructureSpawns.Keys)
            {
                _entitiesFactory.CreateBattleStructure(scenario.StructureSpawns[pos], BattleSide.NoSide, pos);
            }
        }
    }
}
