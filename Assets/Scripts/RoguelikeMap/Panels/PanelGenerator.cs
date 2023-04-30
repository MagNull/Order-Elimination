using System;
using System.Collections.Generic;
using RoguelikeMap.Points;
using StartSessionMenu;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Panels
{
    public class PanelGenerator : MonoBehaviour
    {
        [SerializeField] 
        private Transform _parent;
        private const string Path = "Points\\Panels";

        private List<Panel> _panels = new ();
        private const int ShopPanelIndex = 3;
        private const int SquadMembersPanelIndex = 4;
        public event Action OnInitializedPanels;

        [Inject]
        private void Construct(Wallet wallet)
        {
            OnInitializedPanels += () => SetWalletToShopPanel(wallet);
        }
        
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

        private void SetWalletToShopPanel(Wallet wallet)
        {
            ((ShopPanel)_panels[ShopPanelIndex]).SetWallet(wallet);
        }
    }
}