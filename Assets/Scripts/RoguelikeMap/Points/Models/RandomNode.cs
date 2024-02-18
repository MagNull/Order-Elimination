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

        public int SelectedIndex { get; private set; } = 0;

        public PointModel GetRandomNextPoint()
        {
            var ports = GetPort("exits").GetConnections();
            SelectedIndex = Random.Range(0, ports.Count);
            return GetSelectedNextPoint();
        }

        public PointModel GetSelectedNextPoint()
        {
            var ports = GetPort("exits").GetConnections();
            if (SelectedIndex > ports.Count)
                Debug.LogError("Invalid port index");
            var node = ports[SelectedIndex].node as PointModel;
            return node;
        }
    }
}