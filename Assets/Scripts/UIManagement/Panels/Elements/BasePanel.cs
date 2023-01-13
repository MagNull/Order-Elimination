using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Sirenix.OdinInspector;
using UIManagement.trashToRemove_Mockups;

namespace UIManagement
{
    public class BasePanel : UIPanel
    {
        public override PanelType PanelType => PanelType._BasePanel;
    }
}
