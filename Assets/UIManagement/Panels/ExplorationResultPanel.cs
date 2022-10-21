using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UIManagement.Elements;
using UnityEngine;

public class ExplorationResultPanel : UIPanel, IUIPanel
{
    public override PanelType PanelType => PanelType.ExplorationResult;
    [SerializeField] private PageSwitcher _pageSwitcher;
}
