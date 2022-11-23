using VContainer;

namespace OrderElimination
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private PlanetPoint _target;
        private Squad _squad;
        public PlanetPoint Target => _target;
        public Squad Squad => _squad;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Set(Squad squad, PlanetPoint target)
        {
            _squad = squad;
            _target = target;
        }
        
        public void CreateAttackOrder()
        {
            if(_squad == null || _target == null)
                throw new System.Exception("Characteristics are not set");
            _squad.SetOrder(new AttackOrder(_target, _squad, _objectResolver));
        }

        public void CreateResearchOrder()
        {
            if(_squad == null || _target == null)
                throw new System.Exception("Characteristics are not set");
            _squad.SetOrder(new ResearchOrder(_target, _squad));
        }
    }
}