using Assets.AbilitySystem.PrototypeHelpers;
using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VContainer;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationSceneContext
    {
        public BattleMapView BattleMapView { get; private set; }
        public IParticlesPool ParticlesPool { get; private set; }
        public DefaultAnimationsPool DefaultAnimations { get; private set; }
        public TextEmitter TextEmitter { get; private set; }
        public IReadOnlyEntitiesBank EntitiesBank { get; private set; }

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            BattleMapView = objectResolver.Resolve<BattleMapView>();
            ParticlesPool = objectResolver.Resolve<IParticlesPool>();
            DefaultAnimations = objectResolver.Resolve<DefaultAnimationsPool>();
            EntitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
            TextEmitter = objectResolver.Resolve<TextEmitter>();
        }

        [Inject]
        private AnimationSceneContext(
            BattleMapView battleMapView,
            IParticlesPool particlesPool,
            TextEmitter textEmitter,
            IReadOnlyEntitiesBank entitiesBank)
        {
            BattleMapView = battleMapView;
            ParticlesPool = particlesPool;
            TextEmitter = textEmitter;
            EntitiesBank = entitiesBank;
        }
    }
}
