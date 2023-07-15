using System.Collections.Generic;
using RoguelikeMap.Points;
using RoguelikeMap.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Panels
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField]
        private List<Panel> _panels = new ();
        [ShowInInspector] 
        private const int BattlePanelIndex = 0;
        [ShowInInspector] 
        private const int EventPanelIndex = 1;
        [ShowInInspector] 
        private const int SafeZonePanelIndex = 2;
        [ShowInInspector] 
        private const int ShopPanelIndex = 3;
        
        public Panel GetPanelByPointInfo(PointType pointType)
        {
            return _panels[(int)pointType % _panels.Count];
        }
    }
}