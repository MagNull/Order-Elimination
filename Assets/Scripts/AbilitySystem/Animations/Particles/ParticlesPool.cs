using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace OrderElimination.AbilitySystem.Animations
{
    public class ParticlesPool : SerializedMonoBehaviour, IParticlesPool
    {
        [OdinSerialize]
        private Transform _particlesParent;

        [OdinSerialize, ShowInInspector, AssetsOnly]
        private Dictionary<ParticleType, AnimatedParticle> _parcticlesPrefabs = new();

        private Dictionary<ParticleType, ObjectPool<AnimatedParticle>> _parcticlesPools = new();
        private Dictionary<AnimatedParticle, ParticleType> _particleTypes = new();
        private ParticleType? _currentAwaitedParticle;

        private void Awake()
        {
            _parcticlesPools = new();
            _particleTypes = new();
            foreach (var particleType in _parcticlesPrefabs.Keys)
            {
                var pool = new ObjectPool<AnimatedParticle>(CreateParticle, OnObjectPoolGet, OnObjectPoolRelease);
                _parcticlesPools.Add(particleType, pool);
            }
        }

        private AnimatedParticle CreateParticle()
        {
            if (_currentAwaitedParticle == null || !_currentAwaitedParticle.HasValue)
                throw new InvalidOperationException();
            var prefab = _parcticlesPrefabs[_currentAwaitedParticle.Value];
            var particle = Instantiate(prefab, _particlesParent);
            return particle;
        }

        private void OnObjectPoolGet(AnimatedParticle particle)
        {
            particle.gameObject.SetActive(true);
            particle.transform.SetParent(_particlesParent);
            particle.transform.localPosition = Vector3.zero;
        }

        private void OnObjectPoolRelease(AnimatedParticle particle)
        {
            particle.gameObject.SetActive(false);
        }

        public AnimatedParticle Create(ParticleType parcticleType)
        {
            _currentAwaitedParticle = parcticleType;
            var particle = _parcticlesPools[parcticleType].Get();
            _particleTypes.Add(particle, parcticleType);
            _currentAwaitedParticle = null;
            return particle;
        }

        public void Release(AnimatedParticle particle)
        {
            if (!_particleTypes.ContainsKey(particle))
                throw new ArgumentException("Attempt to release unknown particle.");
            var parcticleType = _particleTypes[particle];
            _parcticlesPools[parcticleType].Release(particle);
            _particleTypes.Remove(particle);
        }
    }
}
