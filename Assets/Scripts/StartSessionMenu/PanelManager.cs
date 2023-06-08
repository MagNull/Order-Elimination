using System.Collections.Generic;
using System.Linq;
using ItemsLibrary;
using RoguelikeMap.UI;
using StartSessionMenu.ChooseCharacter;
using UnityEngine;

namespace StartSessionMenu
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField]
        private ChoosingCharacter _choosingCharacterPanel;
        [SerializeField]
        private MetaShop _metaShopPanel;
        [SerializeField]
        private Panel _libraryPanel;
        [SerializeField] 
        private GameObject _startGameButton;
        [SerializeField]
        private GameObject _wallet;
        [SerializeField]
        private GameObject _startGameTabs;
        private List<Panel> _panels = new();
        
        private void Start()
        {
            _panels.Add(_choosingCharacterPanel);
            _panels.Add(_metaShopPanel);
            _panels.Add(_libraryPanel);
        }

        public void OpenChoosingCharacterPanel() => OpenPanelByType(PanelType.Characters);
        public void OpenMetaShopPanel() => OpenPanelByType(PanelType.MetaShop);
        public void OpenLibraryPanel() => OpenPanelByType(PanelType.Library);

        private void OpenPanelByType(PanelType panelType)
        {
            foreach (var panel in _panels.Where(x => x.IsOpen))
                panel.Close();
            
            switch (panelType)
            {
                case PanelType.MetaShop:
                    _metaShopPanel.Open();
                    _wallet.SetActive(true);
                    _startGameButton.SetActive(true);
                    _startGameTabs.SetActive(true);
                    break;
                case PanelType.Characters:
                    _choosingCharacterPanel.Open();
                    _wallet.SetActive(true);
                    _startGameButton.SetActive(true);
                    _startGameTabs.SetActive(true);
                    break;
                case PanelType.Library:
                    _libraryPanel.Open();
                    _wallet.SetActive(false);
                    _startGameButton.SetActive(false);
                    _startGameTabs.SetActive(false);
                    break;
            }
        }
    }
}
