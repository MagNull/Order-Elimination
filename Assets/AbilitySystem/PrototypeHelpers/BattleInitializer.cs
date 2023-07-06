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

        public void InitiateBattle()
        {
            _battleMapDirector.InitializeMap();
            _entitiesBank.Clear();
        }

        public void StartScenario(BattleScenario scenario)
        {
            var gameAllies = _characterMediator.GetPlayerCharacters().ToArray();
            var gameEnemies = _characterMediator.GetEnemyCharacters().ToArray();
            var allySpawns = scenario.GetAlliesSpawnPositions();
            var enemySpawns = scenario.GetEnemySpawnPositions();
            var structures = scenario.GetStructureSpawns();
            foreach (var pos in structures.Keys)
            {
                _entitiesFactory.CreateBattleStructure(structures[pos], BattleSide.NoSide, pos);
            }
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
        }
    }
}
