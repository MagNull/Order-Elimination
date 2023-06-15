using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace OrderElimination.AbilitySystem.Animations
{
    public class ParticlesPool : SerializedMonoBehaviour, IParticlesPool
    {
        [OdinSerialize]
        private Transform _particlesParent;

        [DictionaryDrawerSettings(KeyLabel = "Particle Type", ValueLabel = "Prefab")]
        [OdinSerialize, ShowInInspector, AssetsOnly]
        private Dictionary<ParticleType, AnimatedParticle> _parcticlesPrefabs = new();

        private Dictionary<ParticleType, ObjectPool<AnimatedParticle>> _parcticlesPools = new();
        private Dictionary<AnimatedParticle, ParticleType> _spawnedParticleTypes = new();
        private ParticleType? _currentAwaitedParticle;

        [Button]
        private void AddMissingParticles()
        {
            foreach (var missingType in EnumExtensions.GetValues<ParticleType>().Except(_parcticlesPrefabs.Keys))
            {
                _parcticlesPrefabs.Add(missingType, null);
            }
        }

        private void Awake()
        {
            _parcticlesPools = new();
            _spawnedParticleTypes = new();
            foreach (var particleType in _parcticlesPrefabs.Keys)
            {
                var pool = new ObjectPool<AnimatedParticle>(CreateParticle, OnObjectPoolGet, OnObjectPoolRelease);
                _parcticlesPools.Add(particleType, pool);
            }
        }

        private AnimatedParticle CreateParticle()
        {
            if (_currentAwaitedParticle == null || !_currentAwaitedParticle.HasValue)
                Logging.LogException( new InvalidOperationException());
            var prefab = _parcticlesPrefabs[_currentAwaitedParticle.Value];
            var particle = Instantiate(prefab, _particlesParent);
            return particle;
        }

        private void OnObjectPoolGet(AnimatedParticle particle)
        {
            particle.gameObject.SetActive(true);
            particle.transform.SetParent(_particlesParent);
            particle.transform.localPosition = Vector3.zero;
            particle.transform.right = Vector3.right;
        }

        private void OnObjectPoolRelease(AnimatedParticle particle)
        {
            particle.gameObject.SetActive(false);
        }

        public AnimatedParticle Create(ParticleType parcticleType)
        {
            _currentAwaitedParticle = parcticleType;
            var particle = _parcticlesPools[parcticleType].Get();
            particle.SetBodyVisibility(true);
            _spawnedParticleTypes.Add(particle, parcticleType);
            _currentAwaitedParticle = null;
            return particle;
        }

        public async UniTask Release(AnimatedParticle particle)
        {
            if (!_spawnedParticleTypes.ContainsKey(particle))
                Logging.LogException( new ArgumentException("Attempt to release unknown particle."));
            var parcticleType = _spawnedParticleTypes[particle];
            particle.SetBodyVisibility(false);
            _spawnedParticleTypes.Remove(particle);
            //await particle trail
            await UniTask.WaitUntil(ParticlesDisappeared);
            particle.StopFollowing();
            _parcticlesPools[parcticleType].Release(particle);

            bool ParticlesDisappeared()
            {
                return !particle.HasTrails;
            }
        }
    }
}
