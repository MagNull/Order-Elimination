using OrderElimination.Infrastructure;
using OrderElimination.MetaGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public interface IReadOnlyEntitiesBank
    {
        public event Action<IReadOnlyEntitiesBank> BankChanged;

        public bool ContainsEntity(AbilitySystemActor entity);
        public AbilitySystemActor[] GetEntities();
        public AbilitySystemActor[] GetEntities(BattleSide side);
        public BattleEntityView GetViewByEntity(AbilitySystemActor entity);
        public AbilitySystemActor GetEntityByView(BattleEntityView view);

        public GameCharacter GetBattleCharacterData(AbilitySystemActor characterEntity);
        public IBattleStructureData GetBattleStructureData(AbilitySystemActor structureEntity);
    }

    public class BattleEntitiesBank : IReadOnlyEntitiesBank
    {
        private readonly Dictionary<AbilitySystemActor, BattleEntityView> _viewsByEntities = new ();
        private readonly Dictionary<BattleEntityView, AbilitySystemActor> _entitiesByViews = new ();
        private readonly Dictionary<AbilitySystemActor, GameCharacter> _basedCharacters = new();
        private readonly Dictionary<AbilitySystemActor, IBattleStructureData> _basedStructures = new();

        public event Action<IReadOnlyEntitiesBank> BankChanged;

        public bool ContainsEntity(AbilitySystemActor entity) => _viewsByEntities.ContainsKey(entity);
        public AbilitySystemActor[] GetEntities() => _viewsByEntities.Keys.ToArray();
        public AbilitySystemActor[] GetEntities(BattleSide side)
            => GetEntities().Where(e => e.BattleSide == side).ToArray();
        public BattleEntityView GetViewByEntity(AbilitySystemActor entity) => _viewsByEntities[entity];
        public AbilitySystemActor GetEntityByView(BattleEntityView view) => _entitiesByViews[view];

        public GameCharacter GetBattleCharacterData(AbilitySystemActor characterEntity)
        {
            if (characterEntity.EntityType != EntityType.Character)
                throw new ArgumentException($"Passed entity is not a {EntityType.Character}.");
            return _basedCharacters[characterEntity];
        }

        public IBattleStructureData GetBattleStructureData(AbilitySystemActor structureEntity)
        {
            if (structureEntity.EntityType != EntityType.Structure)
                throw new ArgumentException($"Passed entity is not a {EntityType.Structure}.");
            return _basedStructures[structureEntity];
        }

        public void AddCharacterEntity(AbilitySystemActor entity, BattleEntityView view, IGameCharacterTemplate basedData)
        {
            if (entity.EntityType != EntityType.Character)
                throw new InvalidOperationException("Attempt to add non-character entity.");
            entity.DisposedFromBattle += OnEntityDisposed;
            _viewsByEntities.Add(entity, view);
            _entitiesByViews.Add(view, entity);
            _basedCharacters.Add(entity, GameCharactersFactory.CreateGameEntity(basedData));
            BankChanged?.Invoke(this);
        }

        public void AddStructureEntity(AbilitySystemActor entity, BattleEntityView view, IBattleStructureData basedData)
        {
            if (entity.EntityType != EntityType.Structure)
                throw new InvalidOperationException("Attempt to add non-structure entity.");
            entity.DisposedFromBattle += OnEntityDisposed;
            _viewsByEntities.Add(entity, view);
            _entitiesByViews.Add(view, entity);
            _basedStructures.Add(entity, basedData);
            BankChanged?.Invoke(this);
        }

        private void OnEntityDisposed(IBattleDisposable entity)
        {
            var entityAsActor = (AbilitySystemActor)entity;
            RemoveEntity(entityAsActor);
        }

        public void RemoveEntity(AbilitySystemActor entity)
        {
            var view = _viewsByEntities[entity];
            //View handles deaths by itsetlf
            //view.DOComplete(true);
            //view.gameObject.SetActive(false); 
            _viewsByEntities.Remove(entity);
            _entitiesByViews.Remove(view);
            _basedCharacters.Remove(entity);
            _basedStructures.Remove(entity);
            entity.DisposedFromBattle -= OnEntityDisposed;
            BankChanged?.Invoke(this);
        }

        public void Clear()
        {
            foreach (var entity in _viewsByEntities.Keys)
                entity.DisposedFromBattle -= OnEntityDisposed;
            _viewsByEntities.Clear();
            _entitiesByViews.Clear();
            _basedCharacters.Clear();
            _basedStructures.Clear();
            BankChanged?.Invoke(this);
        }
    }
}
