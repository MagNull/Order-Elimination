using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIManagement.Debugging;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class SquadListPanel : UIPanel, IUIPanel
    {
        [SerializeField] private CharacterList _characterList;
        public override PanelType PanelType => PanelType.SquadList;
    }
}
