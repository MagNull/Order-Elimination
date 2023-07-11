using System.Collections.Generic;
using System.Linq;
using ItemsLibrary;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.Map;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;
using SquadCommander = RoguelikeMap.SquadInfo.SquadCommander;
using Wallet = StartSessionMenu.Wallet;

//TODO(����): Register Currency
namespace RoguelikeMap
{
    public class RoguelikeMapLifetimeScope : LifetimeScope
    {
        [SerializeField] 
        private int _startMoney = 1000;
        [FormerlySerializedAs("_charactersMediatorPrefab")]
        [SerializeField]
        private ScenesMediator scenesMediatorPrefab;
        [SerializeField]
        private LineRenderer _pathPrefab;
        [SerializeField]
        private Transform _pointsParent;
        [SerializeField]
        private GameObject _pointPrefab;
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

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<ScenesMediator>();
            if (!mediator) 
                mediator = Instantiate(scenesMediatorPrefab);
            else
            {
                mediator.InitTest();
                SquadMediator.SetCharacters(mediator.Get<IEnumerable<GameCharacter>>("player characters").ToList());
            }
            
            var wallet = new Wallet(_startMoney);
            
            builder.Register<Inventory_Items.Inventory>(Lifetime.Singleton).WithParameter(100);
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_squad);
            builder.RegisterComponent(_pathPrefab);
            builder.RegisterComponent(wallet);
            builder.RegisterComponent(_pointPrefab);
            builder.RegisterComponent(_pointsParent);
            builder.RegisterComponent(_library);
            builder.RegisterComponent(_squadMembersPanel);
            builder.RegisterComponent(_characterInfoPanel);
            builder.RegisterComponent(_cardWithHealthBar);
            builder.RegisterComponent(_cardWithCost);
            builder.RegisterComponent(_cardIcon);
            builder.RegisterComponent(_panelManager);
            
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            builder.Register<SimpleMapGenerator>(Lifetime.Singleton).As<IMapGenerator>();
            builder.Register<CharacterCardGenerator>(Lifetime.Singleton);
        }
    }
}