using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class CellTargetGroups: IEnumerable<Cell[]>
    {
        public Cell[] MainTargets { get; }
        public Cell[] AreaTargets { get; }
        public Cell[][] SecondaryTargetGroups { get; }

        public CellTargetGroups(Cell[] mainTargets, Cell[] areaTargets = null, Cell[][] secondaryTargetGroups = null)
        {
            MainTargets = mainTargets;
            AreaTargets = areaTargets;
            SecondaryTargetGroups = secondaryTargetGroups;
        }

        public IEnumerator<Cell[]> GetEnumerator()
        {
            yield return MainTargets;
            yield return AreaTargets;
            foreach (var secondaryTargetGroup in SecondaryTargetGroups)
                yield return secondaryTargetGroup;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
