namespace OrderElimination.AbilitySystem.Animations
{
    public interface IParticlesPool
    {
        public AnimatedParticle Create(ParticleType parcticleType);

        public void Release(AnimatedParticle particle);
    }
}
