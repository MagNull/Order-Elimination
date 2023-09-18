using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    [GUIColor(0.6f, 1, 0.6f)]
    public interface IPointRelativePattern : ICloneable<IPointRelativePattern>
    {
        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint);

        public bool ContainsPositionWithOrigin(Vector2Int position, Vector2Int originPoint);
    }
}
