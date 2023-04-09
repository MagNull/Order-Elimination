using System;
using System.Collections.Generic;
using RoguelikeMap.Points;
using UnityEngine;

namespace RoguelikeMap.Panels
{
    public class PanelGenerator : MonoBehaviour
    {
        [SerializeField] 
        private Transform _parent;
        private const string Path = "Points\\Panels";
        
        private List<IPanel> _panels;

        public event Action OnInitializedPanels;
        
        private void Start()
        {
            _panels = new List<IPanel>();
            var prefabs = Resources.LoadAll<IPanel>(Path);
            InitializePanels(prefabs);
        }
        
        private void InitializePanels(IPanel[] prefabs)
        {
            foreach(var prefab in prefabs)
                _panels.Add(Instantiate(prefab, _parent.position, Quaternion.identity, _parent));
            OnInitializedPanels?.Invoke();
        }
        
        public IPanel GetPanelByPointInfo(PointType pointType)
        {
            return _panels[(int)pointType];
        }
    }
}