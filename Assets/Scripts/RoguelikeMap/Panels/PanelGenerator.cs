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

        private List<Panel> _panels = new List<Panel>();
        private int SquadMembersPanelIndex = 4;
        public event Action OnInitializedPanels;
        
        private void Start()
        {
            var prefabs = Resources.LoadAll<Panel>(Path);
            InitializePanels(prefabs);
        }
        
        private void InitializePanels(Panel[] prefabs)
        {
            foreach(var prefab in prefabs)
                _panels.Add(Instantiate(prefab, _parent.position, Quaternion.identity, _parent));
            OnInitializedPanels?.Invoke();
        }
        
        public Panel GetPanelByPointInfo(PointType pointType)
        {
            return _panels[(int)pointType];
        }

        public SquadMembersPanel GetSquadMembersPanel()
        {
            return (SquadMembersPanel)_panels[SquadMembersPanelIndex];
        }
    }
}