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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

            var defaultProgress = PlayerProgressManager.NewGameProgress;
            var roguelikeMoney = defaultProgress.Currencies[GameCurrency.Roguelike];
            var playerInventory = InventorySerializer.Load();
            if (_loadLocalData
                && PlayerProgressManager.LoadPlayerProgress(out var progress, out var inventory))
            {
                mediator.Register("player characters", progress.PlayerCharacters);
                mediator.Register("stats", progress.StatsUpgrades);
                roguelikeMoney = progress.Currencies[GameCurrency.Roguelike];
                playerInventory = inventory;
                Logging.Log("Player progress data loaded.");
                Logging.Log("Player Currency:\n" + 
                    string.Join("\n", progress.Currencies.Select(c => $"{c.Key}: {c.Value}")));
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

        private void OnApplicationQuit()
        {
            //TODO-SAVE: get and save playerprogress
        }

        private void SaveProgress()
        {
            var sceneMediator = Container.Resolve<ScenesMediator>();
            var inventory = Container.Resolve<Inventory>();
            var roguelikeMoney = Container.Resolve<Wallet>().Money;

            var playerSquad = sceneMediator
                .Get<IEnumerable<GameCharacter>>("player characters")
                .ToArray();
            var upgradeStats = sceneMediator.Get<StrategyStats>("stats");
            var currencies = new Dictionary<GameCurrency, int>()
                {
                    { GameCurrency.Roguelike, roguelikeMoney },
                };
            var progress = new PlayerProgressData(playerSquad, upgradeStats, currencies);

            //var progress = sceneMediator.Get<IPlayerProgressData>("progress");
            PlayerProgressManager.SavePlayerProgress(progress, inventory);
            Logging.Log("Player progress data saved.");
        }
    }
}