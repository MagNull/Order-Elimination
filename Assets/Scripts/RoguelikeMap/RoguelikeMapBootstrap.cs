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
        private IPlayerProgressManager _progressManager;

        private IPlayerProgress Progress => _progressManager.GetPlayerProgress();

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<ScenesMediator>();
            if (!mediator)
            {
                mediator = _testMediator;
                mediator.InitTest();
            }

            var progressManager = mediator.Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager);
            _progressManager = progressManager;
            var playerInventory = Progress.CurrentRunProgress.PlayerInventory;
            var wallet = new Wallet(
                () => Progress.CurrentRunProgress.RunCurrency,
                value => Progress.CurrentRunProgress.RunCurrency = value);
            Logging.Log($"Player money: {wallet.Money}");

            builder.RegisterComponent(wallet);
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

        private void OnDisable()
        {
            //On roguelike scene quit
            _progressManager?.SaveProgress();
        }
    }
}