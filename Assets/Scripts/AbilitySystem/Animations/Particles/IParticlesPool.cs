using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem.Animations
{
    public interface IParticlesPool
    {
        public AnimatedParticle Create(ParticleType parcticleType);

        public UniTask Release(AnimatedParticle particle);
    }
}
