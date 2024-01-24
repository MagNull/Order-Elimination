using GameInventory;
using ItemsLibrary;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
using RoguelikeMap.Map;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;
using SquadCommander = RoguelikeMap.SquadInfo.SquadCommander;
using Wallet = StartSessionMenu.Wallet;

namespace RoguelikeMap
{
    public class RoguelikeMapBootstrap : LifetimeScope, IStartable
    {
        [FormerlySerializedAs("_charactersMediatorPrefab")]
        [SerializeField]
        private ScenesMediator _testMediator;
        [SerializeField]
        private LineRenderer _pathPrefab;
        [SerializeField]
        private Transform _pointsParent;
        [SerializeField]
        private Point _pointPrefab;
        [SerializeField]
        private Squad _squad;
        [SerializeField]
        private PanelManager _panelManager;
        [SerializeField]
        private Library _library;
        [SerializeField]
        private SquadMembersPanel _squadMembersPanel;
        [SerializeField] 
        private CharacterInfoPanel _characterInfoPanel;
        [SerializeField]
        private TransferPanel _transferPanel;
        [SerializeField]
        private CharacterCardWithHealthBar _cardWithHealthBar;
        [SerializeField]
        private CharacterCardWithCost _cardWithCost;
        [SerializeField]
        private CharacterCard _cardIcon;
        [SerializeField]
        private Map.Map _map;

        [Header("Saves Management")]
        [SerializeField]
        private bool _saveLocalData;
        [SerializeField]
        private bool _loadLocalData;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<ScenesMediator>();
            if (!mediator)
            {
                mediator = _testMediator;
                mediator.InitTest();
            }

            var roguelikeMoney = 1800;//default
            var playerInventory = InventorySerializer.Load();
            if (_loadLocalData)
            {
                var progress = mediator.Contains<IPlayerProgress>("progress")
                    ? mediator.Get<IPlayerProgress>("progress")
                    : PlayerProgressManager.LoadSavedProgress();
                if (progress.CurrentRunProgress == null)
                    throw new ArgumentException("Current run progress should be already assigned");
                roguelikeMoney = progress.CurrentRunProgress.RoguelikeCurrency;
                //playerInventory = inventory;
                Logging.Log("Player progress data loaded.");
            }
            Logging.Log($"Player money: {roguelikeMoney}");
            builder.Register<Wallet>(Lifetime.Singleton).WithParameter(roguelikeMoney);
            builder.RegisterComponent(playerInventory);
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_squad);
            builder.RegisterComponent(_pathPrefab);
            builder.RegisterComponent(_pointPrefab);
            builder.RegisterComponent(_pointsParent);
            builder.RegisterComponent(_library);
            builder.RegisterComponent(_squadMembersPanel);
            builder.RegisterComponent(_characterInfoPanel);
            builder.RegisterComponent(_cardWithHealthBar);
            builder.RegisterComponent(_cardWithCost);
            builder.RegisterComponent(_cardIcon);
            builder.RegisterComponent(_panelManager);
            builder.RegisterComponent(_transferPanel);
            builder.RegisterComponent(_map);

            builder.Register<BattleRewardHandler>(Lifetime.Singleton);
            builder.Register<SquadPositionSaver>(Lifetime.Singleton);
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            builder.Register<SimpleMapGenerator>(Lifetime.Singleton).As<IMapGenerator>();
            builder.Register<CharacterCardGenerator>(Lifetime.Singleton);
        }

        public void Start()
        {
            Container.Resolve<BattleRewardHandler>().Start();
        }

        public void OnDisable()
        {
            if (_saveLocalData)
            {
                SaveProgress();
            }
        }

        private void SaveProgress()
        {
            var sceneMediator = Container.Resolve<ScenesMediator>();
            var progress = sceneMediator.Get<IPlayerProgress>("progress");
            PlayerProgressManager.SaveProgress(progress);
        }
    }
}