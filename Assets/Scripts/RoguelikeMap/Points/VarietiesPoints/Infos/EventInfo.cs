using System;
using System.Collections.Generic;
using System.Net.Mime;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class EventInfo
    {
        [SerializeField]
        [HideIf("_isEnd")]
        private string _text;
        
        [SerializeField] 
        [HideIf("_isFork")]
        private bool _isEnd;
        
        [SerializeField]
        [HideIf("_isEnd")]
        private bool _isFork;
        
        [SerializeField]
        [ShowIf("_isEnd")]
        private bool _isHaveItems;
        
        [SerializeField]
        [ShowIf("@this._isEnd && this._isHaveItems")]
        private List<int> _itemsId;

        [SerializeReference]
        [HideIf("@this._isFork || this._isEnd")]
        private EventInfo _nextStage;
        
        [ShowIf("_isFork")]
        [TabGroup("Answers")]
        [SerializeReference]
        private List<string> _answers;
        
        [ShowIf("_isFork")]
        [TabGroup("NextStages")]
        [SerializeReference]
        private List<EventInfo> _nextStages;

        public IReadOnlyList<int> ItemsId => _itemsId;
        public IReadOnlyList<string> Answers => _answers;
        public IReadOnlyList<EventInfo> NextStages => _nextStages;
        public EventInfo NextStage => _nextStage;
        public bool IsHaveItems => _itemsId is not null;
        public bool IsEnd => _isEnd;
        public bool IsFork => _isFork;
        public string Text => _text;
    }
}