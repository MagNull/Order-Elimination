using OrderElimination.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationSceneContext
    {
        public BattleMapView BattleMapView { get; private set; }
        public Vector2 CellSize => BattleMapView.CellSize;
        public float ScaleMultiplier => Mathf.Min(CellSize.x, CellSize.y);
        public IReadOnlyEntitiesBank EntitiesBank { get; private set; }
        public Scene CurrentScene { get; private set; }

        public TextEmitter TextEmitter { get; private set; }
        public SpriteEmitter SpriteEmitter { get; private set; }
        public SoundEffectsPlayer SoundEffectsPlayer { get; private set; }
        public IParticlesPool ParticlesPool { get; private set; }
        public DefaultAnimationsPool DefaultAnimations { get; private set; }

        public Action<AnimationSceneContext> AllAnimationsStopRequested;

        [Inject]
        private AnimationSceneContext(
            BattleMapView battleMapView,
            IParticlesPool particlesPool,
            TextEmitter textEmitter,
            SpriteEmitter spriteEmitter,
            IReadOnlyEntitiesBank entitiesBank,
            DefaultAnimationsPool defaultAnimationsPool,
            SoundEffectsPlayer soundPlayer)
        {
            BattleMapView = battleMapView;
            ParticlesPool = particlesPool;
            TextEmitter = textEmitter;
            SpriteEmitter = spriteEmitter;
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
