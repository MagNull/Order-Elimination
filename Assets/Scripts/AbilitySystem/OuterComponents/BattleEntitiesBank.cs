using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IReadOnlyEntitiesBank
    {
        public event Action<IReadOnlyEntitiesBank> BankChanged;

        public bool ContainsEntity(AbilitySystemActor entity, bool includeDisposed = false);
        public bool ContainsCharacter(GameCharacter character, bool includeDisposed = false);
        public AbilitySystemActor[] GetActiveEntities();
        public AbilitySystemActor[] GetActiveEntities(BattleSide side);
        public AbilitySystemActor[] GetDisposedEntities();

        public BattleEntityView GetViewByEntity(AbilitySystemActor entity);
        public AbilitySystemActor GetEntityByView(BattleEntityView view);

        public GameCharacter GetBasedCharacter(AbilitySystemActor characterEntity);
        public AbilitySystemActor GetEntityByBasedCharacter(GameCharacter gameCharacter);
        public IBattleStructureTemplate GetBasedStructureTemplate(AbilitySystemActor structureEntity);
    }

    public class BattleEntitiesBank : IReadOnlyEntitiesBank
    {
        private readonly HashSet<AbilitySystemActor> _activeEntities = new();
        private readonly HashSet<AbilitySystemActor> _disposedEntities = new();
        private readonly Dictionary<AbilitySystemActor, BattleEntityView> _viewsByEntities = new ();
        private readonly Dictionary<BattleEntityView, AbilitySystemActor> _entitiesByViews = new ();
        private readonly Dictionary<AbilitySystemActor, GameCharacter> _basedCharacters = new();
        private readonly Dictionary<GameCharacter, AbilitySystemActor> _entitiesByCharacters = new();
        private readonly Dictionary<AbilitySystemActor, IBattleStructureTemplate> _basedStructures = new();

        public event Action<IReadOnlyEntitiesBank> BankChanged;

        public bool ContainsEntity(AbilitySystemActor entity, bool includeDisposed = false)
        {
            if (!includeDisposed)
                return _activeEntities.Contains(entity);
            return _activeEntities.Contains(entity) || _disposedEntities.Contains(entity);
        }
        public bool ContainsCharacter(GameCharacter character, bool includeDisposed = false)
        {
            if (!_entitiesByCharacters.ContainsKey(character)) return false;
            var entity = _entitiesByCharacters[character];
            return !entity.IsDisposedFromBattle || includeDisposed;
        }
        public AbilitySystemActor[] GetActiveEntities() => _activeEntities.ToArray();
        public AbilitySystemActor[] GetActiveEntities(BattleSide side)
            => GetActiveEntities().Where(e => e.BattleSide == side).ToArray();
        public AbilitySystemActor[] GetDisposedEntities() => _disposedEntities.ToArray();

        public BattleEntityView GetViewByEntity(AbilitySystemActor entity) => _viewsByEntities[entity];
        public AbilitySystemActor GetEntityByView(BattleEntityView view) => _entitiesByViews[view];

        public GameCharacter GetBasedCharacter(AbilitySystemActor characterEntity)
        {
            if (characterEntity.EntityType != EntityType.Character)
                Logging.LogException(new ArgumentException($"Passed entity is not a {EntityType.Character}."));
            return _basedCharacters[characterEntity];
        }

        public AbilitySystemActor GetEntityByBasedCharacter(GameCharacter gameCharacter)
            => _entitiesByCharacters[gameCharacter];

        public IBattleStructureTemplate GetBasedStructureTemplate(AbilitySystemActor structureEntity)
        {
            if (structureEntity.EntityType != EntityType.Structure)
                Logging.LogException(new ArgumentException($"Passed entity is not a {EntityType.Structure}."));
            return _basedStructures[structureEntity];
        }

        #region Modification
        public void AddCharacterEntity(AbilitySystemActor entity, BattleEntityView view, GameCharacter basedCharacter)
        {
            if (entity.EntityType != EntityType.Character)
                Logging.LogException( new InvalidOperationException("Attempt to add non-character entity."));
            entity.DisposedFromBattle += OnEntityDisposed;
            _activeEntities.Add(entity);
            _viewsByEntities.Add(entity, view);
            _entitiesByViews.Add(view, entity);
            _basedCharacters.Add(entity, basedCharacter);
            _entitiesByCharacters.Add(basedCharacter, entity);
            BankChanged?.Invoke(this);
        }

        public void AddStructureEntity(AbilitySystemActor entity, BattleEntityView view, IBattleStructureTemplate basedData)
        {
            if (entity.EntityType != EntityType.Structure)
                Logging.LogException( new InvalidOperationException("Attempt to add non-structure entity."));
            entity.DisposedFromBattle += OnEntityDisposed;
            _activeEntities.Add(entity);
            _viewsByEntities.Add(entity, view);
            _entitiesByViews.Add(view, entity);
            _basedStructures.Add(entity, basedData);
            BankChanged?.Invoke(this);
        }

        public void RemoveEntity(AbilitySystemActor entity)
        {
            var view = _viewsByEntities[entity];
            if (entity.EntityType == EntityType.Character)
            {
                var character = _basedCharacters[entity];
                _entitiesByCharacters.Remove(character, out var removedEntity);
            }
            //View handles deaths by itsetlf
            //view.DOComplete(true);
            //view.gameObject.SetActive(false); 
            _activeEntities.Remove(entity);
            _disposedEntities.Remove(entity);
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
            _activeEntities.Clear();
            _disposedEntities.Clear();
            _viewsByEntities.Clear();
            _entitiesByViews.Clear();
            _basedCharacters.Clear();
            _entitiesByCharacters.Clear();
            _basedStructures.Clear();
            BankChanged?.Invoke(this);
        }
        #endregion

        private void OnEntityDisposed(IBattleDisposable entity)
        {
            var entityAsActor = (AbilitySystemActor)entity;
            _activeEntities.Remove(entityAsActor);
            _disposedEntities.Add(entityAsActor);
            entity.DisposedFromBattle -= OnEntityDisposed;
            BankChanged?.Invoke(this);
            //RemoveEntity(entityAsActor);
        }
    }
}
