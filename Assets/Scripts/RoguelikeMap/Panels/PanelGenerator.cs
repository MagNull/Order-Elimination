using System;
using System.Collections.Generic;
using RoguelikeMap.Points;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RoguelikeMap.Panels
{
    public class PanelGenerator : MonoBehaviour
    {
        [SerializeField] 
        private Transform _parent;
        [SerializeField]
        private List<Panel> _panelsPrefabs;

        private List<Panel> _panels = new ();
        private const int SquadMembersPanelIndex = 4;
        private IObjectResolver _resolver;

        public event Action OnInitializedPanels;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        private void Start()
        {
            InitializePanels();
        }
        
        private void InitializePanels()
        {
            foreach(var prefab in _panelsPrefabs)
                _panels.Add(_resolver.Instantiate(prefab, _parent.position, Quaternion.identity, _parent));
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