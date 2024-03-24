using System;
using OrderElimination;
using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using XNode;

namespace Events
{
    [Serializable]
    public class Empty { }

    [NodeWidth(300)]
    public class EventNode : IEventNode
    {
        [Input] public Empty entries;
        [Output] public Empty exits;

        [SerializeField]
        private Sprite _image;

        [SerializeField]
        private bool _isPlaySound;

        [SerializeField, ShowIf("_isPlaySound")]
        private AudioResource _sound;

        [SerializeField, MultiLineProperty, TextArea(10, 100)]
        protected string text;

        public override object GetValue(NodePort port)
        {
            return this;
        }

        public override void Process(EventPanel panel, int index = 0)
        {
            NodePort exitPort = GetOutputPort("exits");

            if (!exitPort.IsConnected)
            {
                Debug.LogWarning("Node isn't connected");
                return;
            }

            var node = exitPort.Connection.node as IEventNode;
            node.OnEnter(panel);
        }

        public override void OnEnter(EventPanel panel)
        {
            var eventGraph = graph as EventPointGraph;
            eventGraph.currentNode = this;

            panel.UpdateText(text);
            panel.UpdateSprite(_image);

            if (_isPlaySound && _sound != null)
            {
                if (_sound == null)
                {
                    Logging.LogError("Sound is not set on event node");
                    return;
                }
                panel.PlaySound(_sound);
            }
        }
    }
}