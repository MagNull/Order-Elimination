using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellGroupsContainer
    {
        public static CellGroupsContainer Empty { get; } 

        static CellGroupsContainer()
        {
            Empty = new CellGroupsContainer(new Dictionary<int, Vector2Int[]>());
        }

        private readonly IReadOnlyDictionary<int, Vector2Int[]> _cellGroups;

        public IEnumerable<int> ContainedCellGroups => _cellGroups.Keys;
        public bool ContainsGroup(int group) => _cellGroups.ContainsKey(group);
        public Vector2Int[] GetGroup(int group) => _cellGroups[group];

        public CellGroupsContainer(IReadOnlyDictionary<int, Vector2Int[]> cellGroups)
        {
            if (cellGroups == null)
                Logging.LogException( new ArgumentException());
            _cellGroups = cellGroups;
        }

        public CellGroupsContainer Filter(Func<Vector2Int, bool> posPredicate)
        {
            var filteredGroups = new Dictionary<int, Vector2Int[]>();
            foreach (var cellGroup in ContainedCellGroups)
            {
                filteredGroups.Add(
                    cellGroup, 
                    GetGroup(cellGroup).Where(g => posPredicate(g)).ToArray());
            }
            return new CellGroupsContainer(filteredGroups);
        }

        public override bool Equals(object obj) => Equals(obj as CellGroupsContainer);

        public bool Equals(CellGroupsContainer other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            //if (GetType() != other.GetType()) return false;
            if (_cellGroups.Count != other._cellGroups.Count) return false;
            foreach (var id in _cellGroups.Keys)
            {
                if (!other._cellGroups.ContainsKey(id)) return true;
                //Unoptimized. Sort on creation?
                if (_cellGroups[id].Length != _cellGroups[id].Length) return false;
                var currentCells = _cellGroups[id].OrderBy(e => e.x).ThenBy(e => e.y).ToArray();
                var otherCells = other._cellGroups[id].OrderBy(e => e.x).ThenBy(e => e.y).ToArray();
                for (var i = 0; i < currentCells.Length; i++)
                {
                    if (currentCells[id] != otherCells[i]) return false;
                }
            }
            return true;
        }

        public static bool operator==(CellGroupsContainer cellGroupsA, CellGroupsContainer cellGroupsB)
        {
            if (cellGroupsA is not null)
                return cellGroupsA.Equals(cellGroupsB);
            return cellGroupsB is null;
        }

        public static bool operator !=(CellGroupsContainer cellGroupsA, CellGroupsContainer cellGroupsB)
        {
            return !(cellGroupsA == cellGroupsB);
        }

        //TODO учитывать CellGroupDistributionPolicy
        //public static CellGroupsContainer Add(CellGroupsContainer container1, CellGroupsContainer container2)
        //{
        //    var combinedCellGroups = container1.CellGroups.ToDictionary(e => e.Key, e => e.Value.ToArray());
        //    foreach (var gId in container2.CellGroups.Keys)
        //    {
        //        if (combinedCellGroups.ContainsKey(gId))
        //            combinedCellGroups[gId] = combinedCellGroups[gId].Concat(container2.CellGroups[gId].ToArray()).ToArray();
        //        else
        //            combinedCellGroups.Add(gId, container2.CellGroups[gId].ToArray());
        //    }
        //    return new CellGroupsContainer(combinedCellGroups);
        //}

        //public static CellGroupsContainer operator +(CellGroupsContainer container1, CellGroupsContainer container2)
        //    => Add(container1, container2);
    }
}
