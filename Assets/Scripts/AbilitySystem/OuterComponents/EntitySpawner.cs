using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;
using UnityEngine;
using VContainer;

namespace OrderElimination.AbilitySystem
{
    public class EntitySpawner
    {
        private Lazy<IBattleContext> _battleContext;
        private Lazy<BattleEntitiesFactory> _entitiesFactory;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _entitiesFactory = new Lazy<BattleEntitiesFactory>(() => objectResolver.Resolve<BattleEntitiesFactory>());
            _battleContext = new Lazy<IBattleContext>(() => objectResolver.Resolve<IBattleContext>());
        }

        public AbilitySystemActor SpawnCharacter(
            IGameCharacterTemplate characterTemplate, 
            BattleSide side,
            Vector2Int position)
        {
            var battleMap = _battleContext.Value.BattleMap;
            if (!battleMap.CellRangeBorders.Contains(position))
                Logging.LogException( new ArgumentOutOfRangeException("Position is outside of the map borders."));
            var gameCharacter = GameCharactersFactory.CreateGameEntity(characterTemplate);
            return _entitiesFactory.Value.CreateBattleCharacter(gameCharacter, side, position).Model;
        }

        public AbilitySystemActor SpawnStructure(
            IBattleStructureTemplate structureData, 
            BattleSide side,
            Vector2Int position)
        {
            var battleMap = _battleContext.Value.BattleMap;
            if (!battleMap.CellRangeBorders.Contains(position))
                Logging.LogException( new ArgumentOutOfRangeException("Position is outside of the map borders."));
            return _entitiesFactory.Value.CreateBattleStructure(structureData, side, position).Model;
        }
    }
}
