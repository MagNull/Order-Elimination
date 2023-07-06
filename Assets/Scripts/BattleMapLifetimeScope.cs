using System.ComponentModel.Design;
using Assets.AbilitySystem.PrototypeHelpers;
using DefaultNamespace;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OrderElimination
{
    public class BattleMapLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private BattleEntitiesFactory _battleEntitiesFactory;
        [SerializeField]
        private BattleLoopManager _battleLoopManager;
        [SerializeField]
        private BattleMapView _battleMapView; 
        [SerializeField]
        private ParticlesPool _particlesPool;
        [SerializeField]
        private DefaultAnimationsPool _defaultAnimationsPool;
        [SerializeField]
        private TextEmitter _textEmitter;
        [SerializeField]
        private SoundEffectsPlayer _soundEffectsPlayer;
        [SerializeField]
        private BattleMap _battleMap;
        [SerializeField]
        private BattleMapDirector _battleMapDirector;

        protected override void Configure(IContainerBuilder builder)
        {
            var mediator = FindObjectOfType<CharactersMediator>();
            if (!mediator)
                Logging.LogException( new CheckoutException("No character mediator found"));
            builder.RegisterInstance(new BattleEntitiesBank()).AsSelf().AsImplementedInterfaces();

            builder.RegisterComponent(mediator);
            builder.RegisterComponent(_battleMapDirector);
            builder.RegisterComponent(_battleMapView);
            builder.RegisterComponent(_particlesPool).AsImplementedInterfaces();
            builder.RegisterComponent(_defaultAnimationsPool);
            builder.RegisterComponent(_textEmitter);
            builder.RegisterComponent(_soundEffectsPlayer);
            builder.RegisterComponent(_battleEntitiesFactory);
            builder.RegisterComponent(_battleLoopManager);
            builder.RegisterComponent(_battleMap).As<IBattleMap>();

            builder.Register<SceneTransition>(Lifetime.Singleton);

            builder.Register<BattleInitializer>(Lifetime.Singleton);
            builder.Register<AnimationSceneContext>(Lifetime.Singleton);
            builder.Register<BattleContext>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntitySpawner>(Lifetime.Singleton);
        }
    }
}