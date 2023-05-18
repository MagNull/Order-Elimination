using System;
using System.Collections.Generic;
using RoguelikeMap.Points;
using RoguelikeMap.UI;
using RoguelikeMap.UI.Abilities;
using RoguelikeMap.UI.Characters;
using Sirenix.OdinInspector;
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
        private IObjectResolver _resolver;
        
        [ShowInInspector] 
        private const int BattlePanelIndex = 0;
        [ShowInInspector] 
        private const int EventPanelIndex = 1;
        [ShowInInspector] 
        private const int SafeZonePanelIndex = 2;
        [ShowInInspector] 
        private const int ShopPanelIndex = 3;
        [ShowInInspector] 
        private const int SquadMembersPanelIndex = 4;
        [ShowInInspector]
        private const int CharacterInfoPanelIndex = 5;
        [ShowInInspector] 
        private const int AbilityPanelIndex = 6;

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

        public CharacterInfoPanel GetCharacterInfoPanel()
        {
            return (CharacterInfoPanel)_panels[CharacterInfoPanelIndex];
        }

        public PassiveAbilityInfoPanel GetAbilityPanel()
        {
            return (PassiveAbilityInfoPanel)_panels[AbilityPanelIndex];
        }
    }
}