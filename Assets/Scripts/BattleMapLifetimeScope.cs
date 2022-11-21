using System.ComponentModel.Design;
using CharacterAbility;
using OrderElimination.BattleMap;
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
        //TODO(Sano): Make normal prefab info
        [SerializeField]
        private GameObject _environmentPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if(!mediator)
                throw new CheckoutException("No character mediator found");
            
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_battleCharacterFactory);
            builder.RegisterComponent(_battleMapDirector);


            builder.Register<AbilityFactory>(Lifetime.Singleton).WithParameter(_battleMapView);
            builder.Register<EnvironmentFactory>(Lifetime.Singleton).WithParameter(_environmentPrefab);
            builder.Register<CharacterArrangeDirector>(Lifetime.Singleton);
        }
    }
}