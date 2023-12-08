using System.Collections.Generic;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System.Linq;
using VContainer;
using OrderElimination.Battle;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UIElements;

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

        public void InitiateBattle(int mapWidth, int mapHeight)
        {
            _battleMapDirector.InitializeMap(mapWidth, mapHeight);
            _entitiesBank.Clear();
        }

        public void StartScenario(IBattleMapLayout mapLayout)
        {
            var gameAllies = _sceneMediator
                .Get<IEnumerable<GameCharacter>>("player characters")
                .ToArray();
            var gameEnemies = _sceneMediator
                .Get<IEnumerable<GameCharacter>>("enemy characters")
                .ToArray();
            foreach (var data in mapLayout.GetStructures())
            {
                _entitySpawner.SpawnStructure(data.StructureTemplate, data.Side, data.Position);
            }
            foreach (var data in mapLayout.GetCharacters())
            {
                _entitySpawner.SpawnCharacter(data.CharacterTemplate, data.Side, data.Position);
            }
            var spawns = mapLayout.GetSpawns();
            var allySpawns = spawns.Where(s => s.SpawningSides[BattleSide.Player]).ToArray();
            var enemySpawns = spawns.Except(allySpawns).Where(s => s.SpawningSides[BattleSide.Enemies]).ToArray();
            for (var i = 0; i < gameAllies.Length; i++)
            {
                var entity = gameAllies[i];
                var position = allySpawns[i].Position;
                _entitySpawner.SpawnCharacter(entity, BattleSide.Player, position);
            }
            for (var i = 0; i < gameEnemies.Length; i++)
            {
                var entity = gameEnemies[i];
                var position = enemySpawns[i].Position;
                _entitySpawner.SpawnCharacter(entity, BattleSide.Enemies, position);
            }
        }
    }
}
