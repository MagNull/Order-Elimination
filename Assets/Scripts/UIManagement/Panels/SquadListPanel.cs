using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIManagement.Debugging;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class SquadListPanel : MonoBehaviour, IUIPanel, IDebuggablePanel<SquadListPanel>
    {
        private GameObject _gameObject;
        [SerializeField] private SquadListPanelElement characterElementPrefab;
        [SerializeField] private List<SquadListPanelElement> characterList = new List<SquadListPanelElement>();
        [SerializeField] private Button _exploreButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private TextMeshProUGUI _exploreItemsFindChance;
        [SerializeField] private TextMeshProUGUI _exploreExperiencePercent;
        [SerializeField] private TextMeshProUGUI _exploreAmbushChance;
        [SerializeField] private TextMeshProUGUI _battleItemsFindChance;
        [SerializeField] private TextMeshProUGUI _battleExperiencePercent;
        [SerializeField] private TextMeshProUGUI _battleEnemyLevel;

        public event Action<IUIPanel> Opened;
        public event Action<IUIPanel> Closed;
        public event Action<OrderPanel> ExploreButtonPressed;
        public event Action<OrderPanel> BattleButtonPressed;

        public string Title => "";
        public PanelType PanelType => PanelType.Order;

        private void Awake()
        {
            _gameObject = gameObject;
        }

        #region ToRemove
        private void Start()
        {
            //var info = new PlanetPointOrderInfo()
            //{
            //    ExploreItemsFindChance = 1,
            //    ExploreAmbushChance = 2,
            //    ExploreExperiencePercent = 3,
            //    BattleItemsFindChance = 4,
            //    BattleEnemyLevel = 5,
            //    BattleExperiencePercent = 6
            //};
            //UpdateOrderInfo(info);

            UpdateOrderInfo(new PlanetPointOrderInfo());
        }
        #endregion

        public void Close()
        {
            _gameObject.SetActive(false);
            Closed?.Invoke(this);
        }

        public void Open()
        {
            _gameObject.SetActive(true);
            Opened?.Invoke(this);
        }

        public void UpdateOrderInfo(PlanetPointOrderInfo orderInfo)
        {
            _exploreItemsFindChance.text = $"{orderInfo.ExploreItemsFindChance}%";
            _exploreExperiencePercent.text = $"{orderInfo.ExploreExperiencePercent}%";
            _exploreAmbushChance.text = $"{orderInfo.ExploreAmbushChance}%";
            _battleItemsFindChance.text = $"{orderInfo.BattleItemsFindChance}%";
            _battleExperiencePercent.text = $"{orderInfo.BattleExperiencePercent}%";
            _battleEnemyLevel.text = $"{orderInfo.BattleEnemyLevel}";
        }

        #region Debugging
        private Action<IUIPanel> openedDebug = (p) => Debug.Log($"{nameof(p.PanelType)}:{p.PanelType} \"{(p as OrderPanel).name}\" opened");
        private Action<IUIPanel> closedDebug = (p) => Debug.Log($"{nameof(p.PanelType)}:{p.PanelType} \"{(p as OrderPanel).name}\" closed");
        private Action<OrderPanel> exploreButtonDebug = (p) => Debug.Log($"{nameof(_exploreButton)} pressed on {nameof(p.PanelType)}:{p.PanelType} \"{p.name}\"");
        private Action<OrderPanel> battleButtonDebug = (p) => Debug.Log($"{nameof(_battleButton)} pressed on {nameof(p.PanelType)}:{p.PanelType} \"{p.name}\"");

        public void StartDebugging()
        {
            Opened += openedDebug;
            Closed += closedDebug;
            ExploreButtonPressed += exploreButtonDebug;
            BattleButtonPressed += battleButtonDebug;
        }

        public void StopDebugging()
        {
            Opened -= openedDebug;
            Closed -= closedDebug;
            ExploreButtonPressed -= exploreButtonDebug;
            BattleButtonPressed -= battleButtonDebug;
        }
        #endregion Debugging
    }
}
