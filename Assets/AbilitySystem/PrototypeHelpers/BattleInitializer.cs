using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System.Linq;
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
            var structures = scenario.GetStructureSpawns();
            foreach (var pos in structures.Keys)
            {
                _entitiesFactory.CreateBattleStructure(structures[pos], BattleSide.NoSide, pos);
            }
        }
    }
}
