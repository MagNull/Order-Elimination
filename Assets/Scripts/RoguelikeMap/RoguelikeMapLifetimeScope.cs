using OrderElimination;
using RoguelikeMap.Map;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wallet = StartSessionMenu.Wallet;

//TODO(����): Register Currency
namespace RoguelikeMap
{
    public class RoguelikeMapLifetimeScope : LifetimeScope
    {
        [SerializeField] 
        private int _startMoney = 1000;
        [SerializeField]
        private CharactersMediator _charactersMediatorPrefab;
        [SerializeField]
        private LineRenderer _pathPrefab;
        [SerializeField]
        private Transform _pointsParent;
        [SerializeField]
        private GameObject _pointPrefab;
        [SerializeField]
        private Squad _squad;
        [SerializeField]
        private PanelGenerator _panelGenerator;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if (!mediator) 
                mediator = Instantiate(_charactersMediatorPrefab);
            
            var wallet = new Wallet(_startMoney);
            
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_squad);
            builder.RegisterComponent(_pathPrefab);
            builder.RegisterComponent(wallet);
            builder.RegisterComponent(_panelGenerator);
            builder.RegisterComponent(_pointPrefab);
            builder.RegisterComponent(_pointsParent);
            
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            builder.Register<SimpleMapGenerator>(Lifetime.Singleton).As<IMapGenerator>();
        }
    }
}