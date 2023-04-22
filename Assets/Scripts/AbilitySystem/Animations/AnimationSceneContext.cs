using Assets.AbilitySystem.PrototypeHelpers;
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
        public ParticlesPool ParticlesPool { get; private set; }
        public BattleEntitiesBank EntitiesBank { get; private set; }

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            BattleMapView = objectResolver.Resolve<BattleMapView>();
            ParticlesPool = objectResolver.Resolve<ParticlesPool>();
            EntitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
        }
    }
}
