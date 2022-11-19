using OrderElimination;
using UnityEngine;
using VContainer;
using VContainer.Unity;

//TODO(Сано): Register Currency
public class StrategyMapLifetimeScope : LifetimeScope
{
    [SerializeField]
    private CharactersMediator _charactersMediatorPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        var mediator = FindObjectOfType<CharactersMediator>();
        if (!mediator) 
            mediator = Instantiate(_charactersMediatorPrefab);
        
        builder.RegisterComponent(mediator);
        builder.Register<SquadCommander>(Lifetime.Singleton);
        builder.Register<SceneTransition>(Lifetime.Singleton);
    }
}