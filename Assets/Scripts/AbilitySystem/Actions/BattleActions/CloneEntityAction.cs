using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CloneEntityAction : BattleAction<CloneEntityAction>, 
        IUndoableBattleAction,
        IUtilizeCellGroupsAction
    {
        public enum CloningType
        {
            UseDefault,//Skip, Base
            Copy,
            Override,
            //Add
        }

        private static readonly List<AbilitySystemActor[]> _spawnedEntities = new();
        private static readonly Dictionary<int, IBattleTrigger> _activeTriggers = new();
        private static readonly Dictionary<IBattleTrigger, int> _perforIdsByTriggers = new();
        private static readonly HashSet<int> _undoneOperations = new();

        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        [ShowInInspector, OdinSerialize]
        [ValidateInput(
            "@false",
            "Following features are not supported or not fully implemented yet:\n"
            + "- Modified BattleStats copying\n"
            + "- Abilities copying\n"
            + "- Effects and Inventory cloning\n"
            + "- Abilities cloning for structures\n")]
        public int SpawnAtCellGroup { get; private set; }

        #region CloningParameters
        [BoxGroup("CloningParameters", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public CloningType StatsCloning { get; set; }

        [BoxGroup("CloningParameters")]
        [ShowIf("@" + nameof(StatsCloning) + "==" + nameof(CloningType) + "." + nameof(CloningType.Override))]
        [ShowInInspector, OdinSerialize]
        public GameCharacterStats OverridedStats { get; private set; }

        //[PropertyTooltip("Not implemented")]
        //[ShowInInspector, OdinSerialize]
        //public CloningType EffectsCloning { get; set; }

        //[PropertyTooltip("Not implemented")]
        //[ShowInInspector, OdinSerialize]
        //public CloningType InventoryCloning { get; set; }

        [BoxGroup("CloningParameters")]
        [ShowInInspector, OdinSerialize]
        public CloningType EnergyPointsCloning { get; set; }

        [BoxGroup("CloningParameters")]
        [ShowIf("@" + nameof(EnergyPointsCloning) + "==" + nameof(CloningType) + "." + nameof(CloningType.Override))]
        [ShowInInspector, OdinSerialize]
        private Dictionary<EnergyPoint, int> _overridedEnergyPoints { get; set; } 
            = EnumExtensions.GetValues<EnergyPoint>().ToDictionary(p => p, p => 0);

        [BoxGroup("CloningParameters")]
        [ShowInInspector, OdinSerialize]
        public CloningType ActiveAbilitiesCloning { get; set; }

        [BoxGroup("CloningParameters")]
        [ShowIf("@" + nameof(ActiveAbilitiesCloning) + "==" + nameof(CloningType) + "." + nameof(CloningType.Override))]
        [ShowInInspector, OdinSerialize]
        public ActiveAbilityBuilder[] OverridedActiveAbilities { get; set; } = new ActiveAbilityBuilder[0];

        [BoxGroup("CloningParameters")]
        [ShowInInspector, OdinSerialize]
        public CloningType PassiveAbilitiesCloning { get; set; }

        [BoxGroup("CloningParameters")]
        [ShowIf("@" + nameof(PassiveAbilitiesCloning) + "==" + nameof(CloningType) + "." + nameof(CloningType.Override))]
        [ShowInInspector, OdinSerialize]
        public PassiveAbilityBuilder[] OverridedPassiveAbilities { get; set; } = new PassiveAbilityBuilder[0];

        //IgnoredItems (avoid abuse)
        #endregion

        #region Side
        [PropertySpace(SpaceBefore = 5, SpaceAfter = 0)]
        [ShowInInspector, OdinSerialize]
        public BattleSideReference SideType { get; private set; }

        [ShowIf("@" + nameof(SideType) + " == " + nameof(BattleSideReference) + "." + nameof(BattleSideReference.Absolute))]
        [ShowInInspector, OdinSerialize]
        public BattleSide AbsoluteSide { get; private set; }
        #endregion

        #region RemoveBy
        [ShowInInspector, OdinSerialize]
        public bool RemoveByTrigger { get; private set; }

        [EnableIf(nameof(RemoveByTrigger))]
        [ShowInInspector, OdinSerialize]
        public IContextTriggerSetup RemoveTrigger { get; private set; }
        #endregion

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public int[] UtilizedCellGroups => new[] { SpawnAtCellGroup };

        public override IBattleAction Clone()
        {
            var clone = new CloneEntityAction();
            clone.SpawnAtCellGroup = SpawnAtCellGroup;
            clone.StatsCloning = StatsCloning;
            clone.EnergyPointsCloning = EnergyPointsCloning;
            clone.ActiveAbilitiesCloning = ActiveAbilitiesCloning;
            clone.PassiveAbilitiesCloning = PassiveAbilitiesCloning;
            //clone.EffectsCloning = EffectsCloning;
            //clone.InventoryCloning = InventoryCloning;
            clone.OverridedStats = new GameCharacterStats(OverridedStats);
            if (_overridedEnergyPoints != null)
            {
                clone._overridedEnergyPoints = _overridedEnergyPoints
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            if (OverridedActiveAbilities != null)
                clone.OverridedActiveAbilities = OverridedActiveAbilities.ToArray();
            if (OverridedPassiveAbilities != null)
                clone.OverridedPassiveAbilities = OverridedPassiveAbilities.ToArray();
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
            var target = useContext.TargetEntity;
            var side = SideType switch
            {
                BattleSideReference.Same => caster.BattleSide,
                BattleSideReference.Absolute => AbsoluteSide,
                //BattleSideReference.Opposite => GetOppositeSide(useContext.ActionMaker.BattleSide),
                //BattleSideReference.Conditional => //take from mapping
                _ => throw new NotImplementedException(),
            };
            var performId = _spawnedEntities.Count;
            Logging.Log($"Clone perform Id: {performId}", Colorize.Purple);
            var spawnedInActionEntities = new List<AbilitySystemActor>();
            foreach (var pos in useContext.CellTargetGroups.GetGroup(SpawnAtCellGroup))
            {
                var clone = MakeClone(target, battleContext, side, pos);
                spawnedInActionEntities.Add(clone);
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

        private AbilitySystemActor MakeClone(
            AbilitySystemActor target,
            IBattleContext battleContext,
            BattleSide side,
            Vector2Int pos)
        {
            var statsCloning = StatsCloning == CloningType.Copy;
            var statsOverride = StatsCloning == CloningType.Override;
            var activeAbilitiesOverride = ActiveAbilitiesCloning == CloningType.Override;
            var passiveAbilitiesOverride = PassiveAbilitiesCloning == CloningType.Override;
            var statsValues = EnumExtensions.GetValues<BattleStat>().ToArray();

            AbilitySystemActor clonedEntity;

            if (target.EntityType == EntityType.Structure)
            {
                var structureTemplate = battleContext.EntitiesBank.GetBasedStructureTemplate(target);
                clonedEntity = battleContext.EntitySpawner.SpawnStructure(structureTemplate, side, pos);
                switch (StatsCloning)
                {
                    case CloningType.UseDefault:
                        break;
                    case CloningType.Copy:
                        clonedEntity.BattleStats.Health = target.BattleStats.Health;
                        clonedEntity.BattleStats.PureArmor = target.BattleStats.PureArmor;
                        //...
                        break;
                    case CloningType.Override:
                        foreach (var stat in statsValues)
                        {
                            clonedEntity.BattleStats[stat].UnmodifiedValue = OverridedStats[stat];
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (target.EntityType == EntityType.Character)
            {
                var original = battleContext.EntitiesBank.GetBasedCharacter(target);
                var stats = StatsCloning switch
                {
                    CloningType.UseDefault => original.CharacterData.GetBaseBattleStats(),
                    CloningType.Copy => new GameCharacterStats(original.CharacterStats),
                    CloningType.Override => new GameCharacterStats(OverridedStats),
                    _ => throw new NotImplementedException(),
                };
                var activeAbilities = ActiveAbilitiesCloning switch
                {
                    CloningType.UseDefault 
                    => original.ActiveAbilities.Select(d => d.BasedBuilder).ToArray(),
                    CloningType.Copy 
                    => throw new NotImplementedException(),
                    CloningType.Override 
                    => OverridedActiveAbilities,
                    _ 
                    => throw new NotImplementedException(),
                };
                var passiveAbilities = PassiveAbilitiesCloning switch
                {
                    CloningType.UseDefault
                    => original.PassiveAbilities.Select(d => d.BasedBuilder).ToArray(),
                    CloningType.Copy
                    => throw new NotImplementedException(),
                    CloningType.Override
                    => OverridedPassiveAbilities,
                    _
                    => throw new NotImplementedException(),
                };
                var clonedCharacter = GameCharactersFactory
                    .CreateGameCharacter(original.CharacterData, stats, activeAbilities, passiveAbilities);
                if (statsCloning)
                    clonedCharacter.CurrentHealth = original.CurrentHealth;

                clonedEntity = battleContext.EntitySpawner.SpawnCharacter(clonedCharacter, side, pos);

                if (statsCloning)
                {
                    //TODO Full BattleStats cloning
                    clonedEntity.BattleStats.Health = target.BattleStats.Health;
                    clonedEntity.BattleStats.PureArmor = target.BattleStats.PureArmor;
                }
            }
            else throw new NotImplementedException();
            var energyPointTypes = EnumExtensions.GetValues<EnergyPoint>();
            switch (EnergyPointsCloning)
            {
                case CloningType.UseDefault:
                    foreach (var p in energyPointTypes)
                    {
                        clonedEntity.SetEnergyPoints(p, battleContext.BattleRules.GetEnergyPointsPerRound(p));
                    }
                    break;
                case CloningType.Copy:
                    foreach (var p in energyPointTypes)
                    {
                        clonedEntity.SetEnergyPoints(p, target.EnergyPoints[p]);
                    }
                    break;
                case CloningType.Override:
                    foreach (var p in energyPointTypes)
                    {
                        if (_overridedEnergyPoints.ContainsKey(p))
                            clonedEntity.SetEnergyPoints(p, _overridedEnergyPoints[p]);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return clonedEntity;
        }
    }
}
