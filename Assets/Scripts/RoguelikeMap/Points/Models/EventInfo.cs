using System;
using System.Collections.Generic;
using GameInventory.Items;
using OrderElimination;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class EventInfo
    {
        [SerializeField] 
        private Sprite sprite;
        
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
        [ShowIf("_isFork")] 
        private bool _isRandom;
        
        [SerializeField]
        [ShowIf("_isEnd")]
        private bool _isHaveItems;

        [SerializeField]
        [HideIf("@this._isEnd || this._isFork")]
        private bool _isBattle;
        
        [SerializeField]
        [ShowIf("@this._isEnd && this._isHaveItems")]
        private List<ItemData> _itemsData;

        [SerializeReference]
        [HideIf("@this._isFork || this._isEnd || this._isBattle")]
        private EventInfo _nextStage;
        
        [ShowIf("@this._isFork && !_isRandom")]
        [TabGroup("Answers")]
        [SerializeReference]
        private List<string> _answers;
        
        [ShowIf("_isFork")]
        [TabGroup("NextStages")]
        [SerializeReference]
        private List<EventInfo> _nextStages;

        [ShowIf("_isBattle")] 
        [SerializeField]
        private List<CharacterTemplate> _enemies;

        public IReadOnlyList<ItemData> ItemsId => _itemsData;
        public IReadOnlyList<string> Answers => _answers;
        public IReadOnlyList<EventInfo> NextStages => _nextStages;
        public IReadOnlyList<IGameCharacterTemplate> Enemies => _enemies;
        public EventInfo NextStage => _nextStage;
        public bool IsHaveItems => _itemsData is not null;
        public bool IsEnd => _isEnd;
        public bool IsFork => _isFork;
        public bool IsBattle => _isBattle;
        public bool IsRandomFork => _isFork && _isRandom;
        public string Text => _text;
        public Sprite Sprite => sprite;
    }
}