using System;
using System.Collections.Generic;
using GameInventory.Items;
using OrderElimination;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventInfo : Node
    {
        [Input, HideIf("@this.IsStart")]
        public EventInfo entries;
        
        [Output, HideIf("@this.IsEnd")]
        public EventInfo exits;
        
        [field: SerializeField]
        public Sprite Sprite { get; private set; }
        
        [field: SerializeField, HideIf("@this.IsEnd || this.IsBattle"), MultiLineProperty]
        public string Text { get; private set; }

        [field: SerializeField, HideIf("@this.IsEnd")]
        public bool IsStart { get; private set; }
        
        [field: SerializeField, HideIf("@this.IsFork || this.IsBattle || this.IsStart")]
        public bool IsEnd { get; private set; }
        
        [field: SerializeField, HideIf("@this.IsEnd || this.IsBattle")]
        public bool IsFork { get; private set; }

        [field: SerializeField, ShowIf("IsFork")]
        public bool IsRandom { get; private set; }
        
        [field: SerializeField, ShowIf("IsEnd")]
        public bool IsHaveItems { get; private set; }

        [field: SerializeField, HideIf("@this.IsEnd || this.IsFork")]
        public bool IsBattle { get; private set; }
        
        [SerializeField, ShowIf("@this.IsEnd && this.IsHaveItems")]
        private List<ItemData> _itemsData;

        [TabGroup("Answers")]
        [SerializeReference, ShowIf("@this.IsFork && !IsRandom")]
        private List<string> _answers;

        [SerializeField, ShowIf("IsBattle")]
        private List<CharacterTemplate> _enemies;

        public IReadOnlyList<ItemData> ItemsId => _itemsData;
        public IReadOnlyList<string> Answers => _answers;
        public IReadOnlyList<IGameCharacterTemplate> Enemies => _enemies;
        public bool IsRandomFork => IsFork && IsRandom;
        
        public override object GetValue(NodePort port)
        {
            return this;
        }

        public EventInfo MoveNext(int index)
        {
            var ports = GetPorts();
            if(index > ports.Count)
                Debug.LogError("Invalid port index");
            var nodePort = ports[index];
            return (EventInfo)nodePort.node.GetValue(nodePort);
        }
        
        public EventInfo NextRandomNode()
        {
            var ports = GetPorts();
            var index = Random.Range(0, ports.Count);
            var nodePort = ports[index];
            return (EventInfo)nodePort.node.GetValue(nodePort);
        }

        private List<NodePort> GetPorts()
        {
            var eventGraph = graph as EventPointGraph;

            if (eventGraph.Current != this) {
                Debug.LogWarning("Node isn't active");
                return null;
            }

            return GetOutputPort("exits").GetConnections();
        }
    }
}