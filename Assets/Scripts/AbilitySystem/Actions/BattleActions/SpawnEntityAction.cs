﻿using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class SpawnEntityAction : BattleAction<SpawnEntityAction>, IUndoableBattleAction
    {
        private static readonly List<AbilitySystemActor[]> _spawnedEntities = new();
        private static readonly Dictionary<int, IBattleTrigger> _activeTriggers = new();
        private static readonly Dictionary<IBattleTrigger, int> _perforIdsByTriggers = new();
        private static readonly HashSet<int> _undoneOperations = new();

        public enum SpawningEntityBattleSide
        {
            Same,
            Absolute
        }

        [ShowInInspector, OdinSerialize]
        public EntityType Entity { get; private set; }

        [ShowIf("@" + nameof(Entity) + " == " + nameof(EntityType) + "." + nameof(EntityType.Character))]
        [ShowInInspector, OdinSerialize]
        public IBattleCharacterData CharacterData { get; private set; }

        [ShowIf("@" + nameof(Entity) + " == " + nameof(EntityType) + "." + nameof(EntityType.Structure))]
        [ShowInInspector, OdinSerialize]
        public IBattleStructureData StructureData { get; private set; }

        [ShowInInspector, OdinSerialize]
        public SpawningEntityBattleSide SideType { get; private set; }

        [ShowIf("@" + nameof(SideType) + " == " + nameof(SpawningEntityBattleSide) + "." + nameof(SpawningEntityBattleSide.Absolute))]
        [ShowInInspector, OdinSerialize]
        public BattleSide AbsoluteSide { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int SpawnCellGroup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool RemoveByTrigger { get; private set; }

        [ShowIf(nameof(RemoveByTrigger))]
        [ShowInInspector, OdinSerialize]
        public IContextTriggerSetup RemoveTrigger { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Cell;

        public override IBattleAction Clone()
        {
            var clone = new SpawnEntityAction();
            clone.Entity = Entity;
            clone.CharacterData = CharacterData;
            clone.StructureData = StructureData;
            clone.SideType = SideType;
            clone.AbsoluteSide = AbsoluteSide;
            clone.SpawnCellGroup = SpawnCellGroup;
            clone.RemoveByTrigger = RemoveByTrigger;
            clone.RemoveTrigger = RemoveTrigger;
            return clone;
        }

        public bool IsUndone(int performId) => _undoneOperations.Contains(performId);

        public bool Undo(int performId)
        {
            if (IsUndone(performId)) throw ActionUndoFailedException.AlreadyUndoneException;
            var entities = _spawnedEntities[performId];
            var isSuccessful = true;
            foreach (var entity in entities)
            {
                if (!entity.DisposeFromBattle())
                    isSuccessful = false;
            }
            if (_activeTriggers.ContainsKey(performId))
                _activeTriggers[performId].Deactivate();
            _undoneOperations.Add(performId);
            Debug.Log($"Entities disposed: {isSuccessful}" % Colorize.Purple);
            return isSuccessful;
        }

        public void ClearUndoCache()
        {
            foreach (var trigger in _activeTriggers.Values)
            {
                trigger.Deactivate();
            }
            _activeTriggers.Clear();
            _spawnedEntities.Clear();
            _undoneOperations.Clear();
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var side = SideType switch
            {
                SpawningEntityBattleSide.Same => useContext.ActionMaker.BattleSide,
                SpawningEntityBattleSide.Absolute => AbsoluteSide,
                //SpawningEntityBattleSide.Opposite => GetOppositeSide(useContext.ActionMaker.BattleSide),
                _ => throw new NotImplementedException(),
            };
            var spawnedEntities = new List<AbilitySystemActor>();
            foreach (var pos in useContext.TargetCellGroups.GetGroup(SpawnCellGroup))
            {
                AbilitySystemActor entity;
                switch (Entity)
                {
                    case EntityType.Character:
                        entity = useContext.BattleContext.EntitySpawner.SpawnCharacter(CharacterData, side, pos);
                        break;
                    case EntityType.Structure:
                        entity = useContext.BattleContext.EntitySpawner.SpawnStructure(StructureData, side, pos);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                spawnedEntities.Add(entity);
            }
            _spawnedEntities.Add(spawnedEntities.ToArray());
            var performId = _spawnedEntities.Count - 1;
            Debug.Log($"Spawn perform Id: {performId}" % Colorize.Purple);
            if (RemoveByTrigger)
            {
                var trigger = RemoveTrigger.GetTrigger(useContext.BattleContext);
                _activeTriggers.Add(performId, trigger);
                _perforIdsByTriggers.Add(trigger, performId);
                trigger.Triggered += OnTriggerFired;
                trigger.Activate();
            }
            return new SimpleUndoablePerformResult(this, useContext, true, performId);
        }

        private void OnTriggerFired(ITriggerFireInfo fireInfo)
        {
            fireInfo.Trigger.Triggered -= OnTriggerFired;
            var performId = _perforIdsByTriggers[fireInfo.Trigger];
            Undo(performId);
        }

        private BattleSide GetOppositeSide(BattleSide side)
        {
            return side switch
            {
                BattleSide.NoSide => BattleSide.NoSide,
                BattleSide.Player => BattleSide.Enemies,
                BattleSide.Enemies => BattleSide.Allies,
                BattleSide.Allies => BattleSide.Enemies,
                BattleSide.Others => BattleSide.Allies,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
