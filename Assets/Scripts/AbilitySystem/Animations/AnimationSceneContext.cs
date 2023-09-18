using Assets.AbilitySystem.PrototypeHelpers;
using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationSceneContext
    {
        private Lazy<IBattleContext> _battleContextGetter;

        public IBattleContext BattleContext => _battleContextGetter.Value;
        public BattleMapView BattleMapView { get; private set; }
        public IParticlesPool ParticlesPool { get; private set; }
        public DefaultAnimationsPool DefaultAnimations { get; private set; }
        public TextEmitter TextEmitter { get; private set; }
        public IReadOnlyEntitiesBank EntitiesBank { get; private set; }
        public SoundEffectsPlayer SoundEffectsPlayer { get; private set; }
        public Scene CurrentScene { get; private set; }

        public Action<AnimationSceneContext> AllAnimationsStopRequested;

        [Inject]
        private AnimationSceneContext(
            Lazy<IBattleContext> battleContext,
            BattleMapView battleMapView,
            IParticlesPool particlesPool,
            TextEmitter textEmitter,
            IReadOnlyEntitiesBank entitiesBank,
            DefaultAnimationsPool defaultAnimationsPool,
            SoundEffectsPlayer soundPlayer)
        {
            _battleContextGetter = battleContext;
            BattleMapView = battleMapView;
            ParticlesPool = particlesPool;
            TextEmitter = textEmitter;
            SoundEffectsPlayer = soundPlayer;
            EntitiesBank = entitiesBank;
            DefaultAnimations = defaultAnimationsPool;
            CurrentScene = SceneManager.GetActiveScene();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (CurrentScene == scene)
            {
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
                StopAllPlayingAnimations();
            }
        }

        public void StopAllPlayingAnimations()
        {
            AllAnimationsStopRequested?.Invoke(this);
            Logging.Log("All animations have been cancelled.");
        }
    }
}
