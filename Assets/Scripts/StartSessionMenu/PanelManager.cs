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
        
        private List<Panel> _panels = new();

        private void Start()
        {
            _panels.Add(_choosingCharacterPanel);
            _panels.Add(_metaShopPanel);
            _panels.Add(_libraryPanel);
        }

        public void OpenPanelByType(PanelType panelType)
        {
            foreach (var panel in _panels.Where(x => x.IsOpen))
                panel.Close();
            
            switch (panelType)
            {
                case PanelType.MetaShop:
                    _metaShopPanel.Open();
                    break;
                case PanelType.Characters:
                    _choosingCharacterPanel.Open();
                    break;
                case PanelType.Library:
                    _libraryPanel.Open();
                    break;
            }
        }
    }
}
