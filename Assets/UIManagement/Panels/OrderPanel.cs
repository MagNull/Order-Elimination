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
    public class OrderPanel : UIPanel, IUIPanel, IDebuggablePanel<OrderPanel>
    {
        [SerializeField] private Button _exploreButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private TextMeshProUGUI _exploreItemsFindChance;
        [SerializeField] private TextMeshProUGUI _exploreExperiencePercent;
        [SerializeField] private TextMeshProUGUI _exploreAmbushChance;
        [SerializeField] private TextMeshProUGUI _battleItemsFindChance;
        [SerializeField] private TextMeshProUGUI _battleExperiencePercent;
        [SerializeField] private TextMeshProUGUI _battleEnemyLevel;

        public event Action<OrderPanel> ExploreButtonPressed;
        public event Action<OrderPanel> BattleButtonPressed;

        public override PanelType PanelType => PanelType.Order;

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _exploreButton.onClick.AddListener(OnExploreButtonPressed);
            _battleButton.onClick.AddListener(OnBattleButtonPressed);
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

        public override void Close()
        {
            _gameObject.SetActive(false);
            CallOnClosedEvent();
        }

        public override void Open()
        {
            _gameObject.SetActive(true);
            CallOnOpenedEvent();
        }

        private void OnExploreButtonPressed()
        {
            ExploreButtonPressed?.Invoke(this);
        }

        private void OnBattleButtonPressed()
        {
            BattleButtonPressed?.Invoke(this);
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
        private Action<IUIPanel> openedDebug = (p) => Debug.Log($"{nameof(p.PanelType)}.{p.PanelType} \"{(p as OrderPanel).name}\" opened");
        private Action<IUIPanel> closedDebug = (p) => Debug.Log($"{nameof(p.PanelType)}.{p.PanelType} \"{(p as OrderPanel).name}\" closed");
        private Action<OrderPanel> exploreButtonDebug = (p) => p.ButtonPressedDebug(nameof(_exploreButton));
        private Action<OrderPanel> battleButtonDebug = (p) => p.ButtonPressedDebug(nameof(_battleButton));

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
