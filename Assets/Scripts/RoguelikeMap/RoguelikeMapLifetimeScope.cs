using OrderElimination;
using RoguelikeMap.Panels;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wallet = OrderElimination.Wallet;

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
        private GameObject _pathPrefab;
        [SerializeField]
        private Transform _pointsParent;
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
            
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            
            var mapGenerator = new SimpleMapGenerator(0, _pointsParent);
            mapGenerator.SetPanelGenerator(_panelGenerator);
            builder.RegisterInstance(mapGenerator).As<IMapGenerator>();
        }
    }
}