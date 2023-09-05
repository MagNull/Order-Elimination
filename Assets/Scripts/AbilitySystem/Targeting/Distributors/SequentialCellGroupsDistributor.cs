//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace OrderElimination.AbilitySystem
//{
//    public class SequentialCellGroupsDistributor : ICellGroupsDistributor
//    {
//        //1.Distribution when no targets (caster)
//        //2.Distribution per target range (0-1, 2, 3, ..., others) enums { id, range, others }

//        //1.Distribution when no targets (caster)
//        //2.Fallback target distribution (when no appropriate distribution for target id)
//        //3.Distribution per id

//        [DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Cell Selector")]
//        [ShowInInspector, OdinSerialize]
//        private Dictionary<int, ICellSelector> _groupSelectors = new();

//        [ShowInInspector, OdinSerialize]
//        private float TargetsCount { get; set; }

//        [ShowInInspector, OdinSerialize]
//        private List<BasicCellGroupsDistributor> _distributionsPerTarget = new();

//        public IReadOnlyDictionary<int, ICellSelector> GroupSelectors => _groupSelectors;

//        public CellGroupsContainer DistributeSelection(
//            IBattleContext battleContext,
//            AbilitySystemActor askingEntity,
//            IEnumerable<Vector2Int> selectedPositions)
//        {
//            var targets = selectedPositions.ToArray();
//            var commonSelectorContext = new CellSelectorContext(
//                battleContext, askingEntity, targets);
//            var distributedCells = new Dictionary<int, Vector2Int[]>();
//            for (var i = 0; i < targets.Length; i++)
//            {
//                var targetSelectorContext = new CellSelectorContext(
//                    battleContext, askingEntity, targets[i]);
//            }
//            foreach (var group in _groupSelectors.Keys.OrderBy(x => x))
//            {
//                distributedCells.Add(
//                    group, _groupSelectors[group].GetCellPositions(commonSelectorContext));
//            }
//            return new CellGroupsContainer(distributedCells);
//        }
//    }
//}
