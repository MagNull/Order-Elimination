using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class SpawnEntityAction : BattleAction<SpawnEntityAction>//, IUndoableBattleAction
    {
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
            return clone;
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
            foreach (var pos in useContext.TargetCellGroups.GetGroup(SpawnCellGroup))
            {
                switch (Entity)
                {
                    case EntityType.Character:
                        useContext.BattleContext.EntitySpawner.SpawnCharacter(CharacterData, side, pos);
                        break;
                    case EntityType.Structure:
                        useContext.BattleContext.EntitySpawner.SpawnStructure(StructureData, side, pos);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return new SimplePerformResult(this, useContext, true);
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
