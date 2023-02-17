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
        private Image _attackImage;
        [SerializeField] 
        private Information _playerInformation;
        [SerializeField]
        private Squad _squad;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if (!mediator) 
                mediator = Instantiate(_charactersMediatorPrefab);
        
            builder.RegisterComponent(mediator);
            builder.Register<SquadCommander>(Lifetime.Singleton);
            builder.Register<SceneTransition>(Lifetime.Singleton);
            builder.RegisterComponent(_playerInformation);
            builder.RegisterComponent(_attackImage);
            builder.RegisterComponent(_squad);
        }
    }
}