using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class SpawnEntityAction : BattleAction<SpawnEntityAction>, IUndoableBattleAction
    {
        private static readonly List<List<AbilitySystemActor>> _spawnedEntities = new();
        private static readonly Dictionary<int, IBattleTrigger> _activeTriggers = new();
        private static readonly Dictionary<IBattleTrigger, int> _perforIdsByTriggers = new();
        private static readonly HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        public EntityType Entity { get; private set; }

        [ShowIf("@" + nameof(Entity) + " == " + nameof(EntityType) + "." + nameof(EntityType.Character))]
        [ShowInInspector, OdinSerialize]
        public IGameCharacterTemplate CharacterData { get; private set; }

        [ShowIf("@" + nameof(Entity) + " == " + nameof(EntityType) + "." + nameof(EntityType.Structure))]
        [ShowInInspector, OdinSerialize]
        public IBattleStructureTemplate StructureData { get; private set; }

        [ShowInInspector, OdinSerialize]
        public BattleSideReference SideType { get; private set; }

        [ShowIf("@" + nameof(SideType) + " == " + nameof(BattleSideReference) + "." + nameof(BattleSideReference.Absolute))]
        [ShowInInspector, OdinSerialize]
        public BattleSide AbsoluteSide { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int SpawnAtCellGroup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool RemoveByTrigger { get; private set; }

        //TODO: Add support for entity triggers
        [ShowIf(nameof(RemoveByTrigger))]
        [ShowInInspector, OdinSerialize]
        public IContextTriggerSetup RemoveTrigger { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Maker;

        public override IBattleAction Clone()
        {
            var clone = new SpawnEntityAction();
            clone.Entity = Entity;
            clone.CharacterData = CharacterData;
            clone.StructureData = StructureData;
            clone.SideType = SideType;
            clone.AbsoluteSide = AbsoluteSide;
            clone.SpawnAtCellGroup = SpawnAtCellGroup;
            clone.RemoveByTrigger = RemoveByTrigger;
            clone.RemoveTrigger = RemoveTrigger;
            return clone;
        }

        public bool IsUndone(int performId) => _undoneOperations.Contains(performId);

        public bool Undo(int performId)
        {
            if (IsUndone(performId)) Logging.LogException(ActionUndoFailedException.AlreadyUndoneException);
            var isSuccessful = true;
            foreach (var entity in _spawnedEntities[performId])
            {
                if (!entity.IsDisposedFromBattle && !entity.DisposeFromBattle())
                    isSuccessful = false;
            }
            //TODO: Add support for entity triggers
            if (_activeTriggers.ContainsKey(performId))
                _activeTriggers[performId].Deactivate();
            _undoneOperations.Add(performId);
            Logging.Log($"Entities disposed: {isSuccessful}", Colorize.Purple);
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
            var battleContext = useContext.BattleContext;
            var caster = useContext.ActionMaker;
            var side = SideType switch
            {
                BattleSideReference.Same => caster.BattleSide,
                BattleSideReference.Absolute => AbsoluteSide,
                //SpawningEntityBattleSide.Opposite => GetOppositeSide(useContext.ActionMaker.BattleSide),
                _ => throw new NotImplementedException(),
            };
            var performId = _spawnedEntities.Count;
            Logging.Log($"Spawn perform Id: {performId}", Colorize.Purple);
            _spawnedEntities.Add(new());
            var currentPerformEntities = _spawnedEntities[_spawnedEntities.Count - 1];
            foreach (var pos in useContext.CellTargetGroups.GetGroup(SpawnAtCellGroup))
            {
                var entity = Entity switch
                {
                    EntityType.Character => battleContext.EntitySpawner.SpawnCharacter(CharacterData, side, pos),
                    EntityType.Structure => battleContext.EntitySpawner.SpawnStructure(StructureData, side, pos),
                    _ => throw new NotImplementedException(),
                };
                currentPerformEntities.Add(entity);
            }
            if (RemoveByTrigger)//TODO: Add support for entity triggers
            {
                var trigger = RemoveTrigger.GetTrigger(battleContext);
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
