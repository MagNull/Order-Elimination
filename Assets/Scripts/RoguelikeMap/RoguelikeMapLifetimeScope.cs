using Inventory_Items;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;
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
        private DialogWindow _dialogWindow;
        [SerializeField]
        private Squad _squad;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if (!mediator) 
                mediator = Instantiate(_charactersMediatorPrefab);
            
            var wallet = new Wallet(_startMoney);
            
            builder.Register<Inventory>(Lifetime.Singleton).WithParameter(100);

            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_squad);
            builder.RegisterComponent(_pathPrefab);
            builder.RegisterComponent(_dialogWindow);
            builder.RegisterComponent(wallet);
            
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            // builder.RegisterComponent(_playerInformation);
            
            var mapGenerator = new SimpleMapGenerator(0, _pointsParent);
            builder.RegisterInstance(mapGenerator).As<IMapGenerator>();
        }
    }
}