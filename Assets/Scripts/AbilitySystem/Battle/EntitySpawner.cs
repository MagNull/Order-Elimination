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
            var battleContext = _battleContext.Value;
            var battleMap = battleContext.BattleMap;
            if (!battleMap.CellRangeBorders.Contains(position))
                Logging.LogException( new ArgumentOutOfRangeException("Position is outside of the map borders."));
            var gameCharacter = GameCharactersFactory.CreateGameCharacter(characterTemplate);
            var entity = _entitiesFactory.Value.CreateBattleCharacter(gameCharacter, side, position).Model;
            entity.PassiveAbilities.ForEach(a => a.Activate(battleContext, entity));
            //Activate passive abilities
            return entity;
        }

        public AbilitySystemActor SpawnCharacter(
            GameCharacter gameCharacter,
            BattleSide side,
            Vector2Int position)
        {
            var battleContext = _battleContext.Value;
            var battleMap = battleContext.BattleMap;
            if (!battleMap.CellRangeBorders.Contains(position))
                Logging.LogException(new ArgumentOutOfRangeException("Position is outside of the map borders."));
            if (battleContext.EntitiesBank.ContainsCharacter(gameCharacter))
                throw new InvalidOperationException("Passed GameCharacter already exists in battle.");
            var entity = _entitiesFactory.Value.CreateBattleCharacter(gameCharacter, side, position).Model;
            entity.PassiveAbilities.ForEach(a => a.Activate(battleContext, entity));
            return entity;
        }

        public AbilitySystemActor SpawnStructure(
            IBattleStructureTemplate structureData, 
            BattleSide side,
            Vector2Int position)
        {
            var battleContext = _battleContext.Value;
            var battleMap = battleContext.BattleMap;
            if (!battleMap.CellRangeBorders.Contains(position))
                Logging.LogException( new ArgumentOutOfRangeException("Position is outside of the map borders."));
            var entity = _entitiesFactory.Value.CreateBattleStructure(structureData, side, position).Model;
            entity.PassiveAbilities.ForEach(a => a.Activate(battleContext, entity));
            return entity;
        }
    }
}
