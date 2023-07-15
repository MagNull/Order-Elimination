using GameInventory;
using ItemsLibrary;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
using RoguelikeMap.Map;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
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
        [SerializeField] 
        private int _startMoney = 1000;
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
        private CharacterCardWithHealthBar _cardWithHealthBar;
        [SerializeField]
        private CharacterCardWithCost _cardWithCost;
        [SerializeField]
        private CharacterCard _cardIcon;

        [Header("Saves Management")]
        [SerializeField]
        private CharacterTemplatesMapping _characterTemplatesMapping;
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
            LoadLocalData(mediator, builder);
            builder.Register<Wallet>(Lifetime.Singleton).WithParameter(
                PlayerPrefs.GetInt("Wallet") == 0 ? _startMoney : PlayerPrefs.GetInt("Wallet"));
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

            builder.Register<BattleRewardHandler>(Lifetime.Singleton);
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            builder.Register<SimpleMapGenerator>(Lifetime.Singleton).As<IMapGenerator>();
            builder.Register<CharacterCardGenerator>(Lifetime.Singleton);
        }

        public void Start()
        {
            Container.Resolve<SquadCommander>().Start();
            Container.Resolve<BattleRewardHandler>().Start();
        }

        public void OnDisable()
        {
            var sceneMediator = Container.Resolve<ScenesMediator>();
            var inventory = Container.Resolve<Inventory>();
            var playerSquad = sceneMediator.Get<IEnumerable<GameCharacter>>("player characters");
            var upgradeStats = sceneMediator.Get<StrategyStats>("stats");
            SaveLocalData(playerSquad, inventory, upgradeStats);
        }

        protected void SaveLocalData(
            IEnumerable<GameCharacter> playerSquad, 
            Inventory inventory,
            StrategyStats upgradeStats)
        {
            PlayerPrefs.SetInt("Wallet", Container.Resolve<Wallet>().Money);
            InventorySerializer.Save(inventory);
            if (!_saveLocalData) return;
            LocalDataManager.SaveLocalData(
                playerSquad.ToArray(), upgradeStats, _characterTemplatesMapping);
        }

        protected void LoadLocalData(ScenesMediator mediator, IContainerBuilder containerBuilder)
        {
            var inventory = InventorySerializer.Load();
            containerBuilder.RegisterComponent(inventory);
            
            if (!_loadLocalData) return;
            if (LocalDataManager.IsLocalDataExists)
            {
                var localData = LocalDataManager.LoadLocalData();
                var playerCharacters = localData.PlayerSquadCharacters
                    .Select(d => GameCharacterSerializer.SaveDataToCharacter(d, _characterTemplatesMapping))
                    .ToArray();
                if (playerCharacters.Length == 0)
                {
                    Logging.LogError(new LocalDataCorruptedException("Squad members count is 0"));
                }
                else
                {
                    mediator.Register("player characters", playerCharacters);
                    //localData.PlayerInventory;
                    mediator.Register("stats", localData.StatsUpgrades);
                }
            }
        }

        private void OnApplicationQuit()
        {
            InventorySerializer.Delete();
        }
    }
}