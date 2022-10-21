using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIManagement.Debugging;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class OrderPanel : UIPanel, IUIPanel, IDebuggablePanel<OrderPanel>
    {
        [SerializeField] private Button _exploreButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private IconTextValueList _exploreParameters;
        [SerializeField] private IconTextValueList _battleParameters;

        public event Action<OrderPanel> ExploreButtonPressed;
        public event Action<OrderPanel> BattleButtonPressed;

        public override PanelType PanelType => PanelType.Order;

        protected override void Initialize()
        {
            if (_isInitialized)
                return;
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
            gameObject.SetActive(false);
            CallOnClosedEvent();
        }

        public override void Open()
        {
            gameObject.SetActive(true);
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
            _exploreParameters.Clear();
            _exploreParameters.Add(null, "Предметы", $"{orderInfo.ExploreItemsFindChance}%");
            _exploreParameters.Add(null, "Шанс отпора", $"{orderInfo.ExploreAmbushChance}%");
            _exploreParameters.Add(null, "Кол-во опыта", $"{orderInfo.ExploreExperiencePercent}%");
            _battleParameters.Clear();
            _battleParameters.Add(null, "Предметы", $"{orderInfo.BattleItemsFindChance}%");
            _battleParameters.Add(null, "Уровень врагов", $"{orderInfo.BattleEnemyLevel}%");
            _battleParameters.Add(null, "Кол-во опыта", $"{orderInfo.BattleExperiencePercent}%");
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
