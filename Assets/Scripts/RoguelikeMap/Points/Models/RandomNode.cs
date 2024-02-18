using System.Collections.Generic;
using System.Linq;
using XNode;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [NodeWidth(100)]
    public class RandomNode : Node
    {
        [Input] public PointModel entries;
        [Output] public PointModel exits;

        public PointModel GetRandomNextPoint()
        {
            int index = Random.Range(0, Outputs.First().ConnectionCount);
            var ports = GetPort("exits").GetConnections();
            if (index > ports.Count)
                Debug.LogError("Invalid port index");
            var node = ports[index].node as PointModel;
            return node;
        }

        public IEnumerable<PointModel> GetNextPoints()
        {
            return !HasPort("exits") ? new List<PointModel>()
                : GetPort("exits").GetConnections().Select(connection => connection.node as PointModel);
        }
    }
}