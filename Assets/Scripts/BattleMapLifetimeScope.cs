using System.ComponentModel.Design;
using CharacterAbility;
using OrderElimination.Battle;
using OrderElimination.BM;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OrderElimination
{
    public class BattleMapLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private BattleCharacterFactory _battleCharacterFactory;
        [SerializeField]
        private BattleMapView _battleMapView;
        [SerializeField]
        private BattleMapDirector _battleMapDirector;
        [SerializeField]
        private CharactersBank _charactersBank; 
        //TODO(Sano): Make normal prefab info
        [SerializeField]
        private GameObject _environmentPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if (!mediator)
                throw new CheckoutException("No character mediator found");

            _charactersBank = new CharactersBank();
            builder.RegisterInstance(_charactersBank).AsSelf().AsImplementedInterfaces();
            
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_battleCharacterFactory);
            builder.RegisterComponent(_battleMapDirector);

            builder.Register<SceneTransition>(Lifetime.Singleton);

            builder.Register<AbilityFactory>(Lifetime.Singleton).WithParameter(_battleMapView);
            builder.Register<EnvironmentFactory>(Lifetime.Singleton).WithParameter(_environmentPrefab)
                .WithParameter(_battleMapDirector.Map);
            builder.Register<CharacterArrangeDirector>(Lifetime.Singleton);
        }
    }
}