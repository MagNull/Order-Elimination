using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class EventInfo
    {
        [SerializeField]
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
        [ShowIf("_isHaveItems")]
        private List<int> _itemsId;
        
        [SerializeField]
        [ShowIfGroup("_isFork")]
        public EventPointInfo _isAcceptNextStage, _isDeclineNextStage;
        
        public IReadOnlyList<int> ItemsId => _itemsId;
        public bool IsHaveItems => _itemsId.Count != 0;
        public bool IsEnd => _isEnd;
        public bool IsFork => _isFork;
    }
}