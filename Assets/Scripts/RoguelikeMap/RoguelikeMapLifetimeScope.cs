using OrderElimination;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

//TODO(����): Register Currency
namespace RoguelikeMap
{
    public class RoguelikeMapLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private CharactersMediator _charactersMediatorPrefab;
        [SerializeField]
        private GameObject _pathPrefab;
        [SerializeField]
        private Transform _pointsParent;
        [SerializeField] 
        private DialogWindow _dialogWindow;
        // [SerializeField] 
        // private Information _playerInformation;
        [SerializeField]
        private Squad _squad;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if (!mediator) 
                mediator = Instantiate(_charactersMediatorPrefab);
        
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_squad);
            builder.RegisterComponent(_pathPrefab);
            builder.RegisterComponent(_dialogWindow);
            
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            // builder.RegisterComponent(_playerInformation);
            
            var mapGenerator = new SimpleMapGenerator(0, _pointsParent);
            builder.RegisterInstance(mapGenerator).As<IMapGenerator>();
        }
    }
}