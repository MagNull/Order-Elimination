using VContainer;

namespace OrderElimination
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        public Order CreateAttackOrder(PlanetPoint target, Squad squad)
        {
            return new AttackOrder(target, squad, _objectResolver);
        }

        public Order CreateResearchOrder(PlanetPoint target, Squad squad)
        {
            return new ResearchOrder(target, squad);
        }
    }
}