using System.Collections.Generic;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System.Linq;
using VContainer;

namespace Assets.AbilitySystem.PrototypeHelpers
{
    public class BattleInitializer
    {
        private BattleMapDirector _battleMapDirector;
        private ScenesMediator _sceneMediator;
        private EntitySpawner _entitySpawner;
        private BattleEntitiesBank _entitiesBank;

        [Inject]
        private void Construct(
            BattleMapDirector mapDirector, 
            ScenesMediator scenesMediator, 
            EntitySpawner entitySpawner,
            BattleEntitiesBank entitiesBank)
        {
            _battleMapDirector = mapDirector;
            _sceneMediator = scenesMediator;
            _entitySpawner = entitySpawner;
            _entitiesBank = entitiesBank;
        }

        public void InitiateBattle()
        {
            _battleMapDirector.InitializeMap();
            _entitiesBank.Clear();
        }

        public void StartScenario(BattleScenario scenario)
        {
            var gameAllies = _sceneMediator
                .Get<IEnumerable<GameCharacter>>("player characters")
                .ToArray();
            var gameEnemies = _sceneMediator
                .Get<IEnumerable<GameCharacter>>("enemy characters")
                .ToArray();
            var allySpawns = scenario.GetAlliesSpawnPositions();
            var enemySpawns = scenario.GetEnemySpawnPositions();
            var structures = scenario.GetStructureSpawns();
            foreach (var pos in structures.Keys)
            {
                _entitySpawner.SpawnStructure(structures[pos], BattleSide.NoSide, pos);
            }
            for (var i = 0; i < gameAllies.Length; i++)
            {
                var entity = gameAllies[i];
                var position = allySpawns[i];
                _entitySpawner.SpawnCharacter(entity, BattleSide.Player, position);
            }
            for (var i = 0; i < gameEnemies.Length; i++)
            {
                var entity = gameEnemies[i];
                var position = enemySpawns[i];
                _entitySpawner.SpawnCharacter(entity, BattleSide.Enemies, position);
            }
        }
    }
}
