using System.ComponentModel.Design;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OrderElimination
{
    public class BattleMapLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private BattleCharacterFactory _battleCharacterFactory;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if(!mediator)
                throw new CheckoutException("No character mediator found");
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_battleCharacterFactory);
            builder.Register<CharacterArrangeDirector>(Lifetime.Singleton);
        }
    }
}