using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class CloneTargetEntityAction : BattleAction<CloneTargetEntityAction>, IUndoableBattleAction
    {
        private static readonly List<AbilitySystemActor[]> _spawnedEntities = new();
        private static readonly Dictionary<int, IBattleTrigger> _activeTriggers = new();
        private static readonly Dictionary<IBattleTrigger, int> _perforIdsByTriggers = new();
        private static readonly HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        public int SpawnAtCellGroup { get; private set; }

        #region CloningParameters
        [ShowInInspector, OdinSerialize]
        public bool CloneStats { get; set; }

        [PropertyTooltip("Not implemented")]
        [ShowInInspector, OdinSerialize]
        public bool CloneEffects { get; set; }

        [PropertyTooltip("Not implemented")]
        [ShowInInspector, OdinSerialize]
        public bool CloneInventory { get; set; }

        //IgnoredItems (avoid abuse)
        #endregion

        #region Side
        [ShowInInspector, OdinSerialize]
        public BattleSideReference SideType { get; private set; }

        [ShowIf("@" + nameof(SideType) + " == " + nameof(BattleSideReference) + "." + nameof(BattleSideReference.Absolute))]
        [ShowInInspector, OdinSerialize]
        public BattleSide AbsoluteSide { get; private set; }
        #endregion

        #region Remove
        [ShowInInspector, OdinSerialize]
        public bool RemoveByTrigger { get; private set; }

        [ShowIf(nameof(RemoveByTrigger))]
        [ShowInInspector, OdinSerialize]
        public IContextTriggerSetup RemoveTrigger { get; private set; }
        #endregion

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public override IBattleAction Clone()
        {
            var clone = new CloneTargetEntityAction();
            clone.SpawnAtCellGroup = SpawnAtCellGroup;
            clone.CloneStats = CloneStats;
            clone.CloneEffects = CloneEffects;
            clone.CloneInventory = CloneInventory;
            clone.SideType = SideType;
            clone.AbsoluteSide = AbsoluteSide;
            clone.RemoveByTrigger = RemoveByTrigger;
            clone.RemoveTrigger = RemoveTrigger;
            //...
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
            var target = useContext.ActionTarget;
            var side = SideType switch
            {
                BattleSideReference.Same => caster.BattleSide,
                BattleSideReference.Absolute => AbsoluteSide,
                //SpawningEntityBattleSide.Opposite => GetOppositeSide(useContext.ActionMaker.BattleSide),
                _ => throw new NotImplementedException(),
            };
            var performId = _spawnedEntities.Count;
            Logging.Log($"Clone perform Id: {performId}", Colorize.Purple);
            var spawnedInActionEntities = new List<AbilitySystemActor>();
            foreach (var pos in useContext.TargetCellGroups.GetGroup(SpawnAtCellGroup))
            {
                AbilitySystemActor entity;

                if (useContext.ActionTarget.EntityType == EntityType.Structure)
                {
                    var structureTemplate = battleContext.EntitiesBank.GetBasedStructureTemplate(target);
                    entity = battleContext.EntitySpawner.SpawnStructure(structureTemplate, side, pos);
                }
                else if (useContext.ActionTarget.EntityType == EntityType.Character)
                {
                    var original = battleContext.EntitiesBank.GetBasedCharacter(target);
                    var stats = original.CharacterData.GetBaseBattleStats();
                    if (CloneStats)
                    {
                        stats = new GameCharacterStats(original.CharacterStats);
                    }
                    var clone = GameCharactersFactory.CreateGameCharacter(original.CharacterData, stats);
                    entity = battleContext.EntitySpawner.SpawnCharacter(clone, side, pos);
                    //TODO Full cloning
                    if (CloneStats)
                    {
                        entity.BattleStats.Health = target.BattleStats.Health;
                        entity.BattleStats.PureArmor = target.BattleStats.PureArmor;
                    }
                }
                else throw new NotImplementedException();

                spawnedInActionEntities.Add(entity);
            }
            _spawnedEntities.Add(spawnedInActionEntities.ToArray());
            if (RemoveByTrigger)
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
    }
}
