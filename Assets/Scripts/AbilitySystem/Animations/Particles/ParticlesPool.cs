using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace OrderElimination.AbilitySystem.Animations
{
    public class ParticlesPool : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private Transform _particlesParent;

        [OdinSerialize, ShowInInspector, AssetsOnly]
        private Dictionary<Particles, AnimatedParticle> _parcticlesPrefabs = new();

        private Dictionary<Particles, ObjectPool<AnimatedParticle>> _parcticlesPools = new();
        private Dictionary<AnimatedParticle, Particles> _particleTypes = new();

        private Particles? _currentAwaitedParticle;

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

        public AnimatedParticle Create(Particles parcticleType)
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
