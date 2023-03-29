using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityExecutionGroups: IEnumerable<Vector2Int[]> //TODO Make sturct
    {
        public Vector2Int[] MainTargets { get; }
        public Vector2Int[] AreaTargets { get; }
        public Vector2Int[][] SecondaryTargetGroups { get; }

        public AbilityExecutionGroups(Vector2Int[] mainTargets, Vector2Int[] areaTargets = null, Vector2Int[][] secondaryTargetGroups = null)
        {
            MainTargets = mainTargets;
            AreaTargets = areaTargets;
            SecondaryTargetGroups = secondaryTargetGroups;
        }

        public IEnumerator<Vector2Int[]> GetEnumerator()
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
