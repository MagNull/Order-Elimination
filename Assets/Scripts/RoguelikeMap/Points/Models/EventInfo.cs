using System;
using System.Collections.Generic;
using OrderElimination;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventInfo
    {
        [SerializeField]
        [HideIf("@this._isEnd || this._isBattle")]
        private string _text;
        
        [SerializeField] 
        [HideIf("@this._isFork || this._isBattle")]
        private bool _isEnd;
        
        [SerializeField]
        [HideIf("@this._isEnd || this._isBattle")]
        private bool _isFork;
        
        [SerializeField]
        [ShowIf("_isEnd")]
        private bool _isHaveItems;

        [SerializeField]
        [HideIf("@this._isEnd || this._isFork")]
        private bool _isBattle;
        
        [SerializeField]
        [ShowIf("@this._isEnd && this._isHaveItems")]
        private List<int> _itemsId;

        [SerializeReference]
        [HideIf("@this._isFork || this._isEnd || this._isBattle")]
        private EventInfo _nextStage;
        
        [ShowIf("_isFork")]
        [TabGroup("Answers")]
        [SerializeReference]
        private List<string> _answers;
        
        [ShowIf("_isFork")]
        [TabGroup("NextStages")]
        [SerializeReference]
        private List<EventInfo> _nextStages;

        [ShowIf("_isBattle")] 
        [SerializeField]
        private List<Character> _enemies;

        public IReadOnlyList<int> ItemsId => _itemsId;
        public IReadOnlyList<string> Answers => _answers;
        public IReadOnlyList<EventInfo> NextStages => _nextStages;
        public IReadOnlyList<IBattleCharacterInfo> Enemies => _enemies;
        public EventInfo NextStage => _nextStage;
        public bool IsHaveItems => _itemsId is not null;
        public bool IsEnd => _isEnd;
        public bool IsFork => _isFork;
        public bool IsBattle => _isBattle;
        public string Text => _text;
    }
}