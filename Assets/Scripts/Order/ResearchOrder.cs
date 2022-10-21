 namespace OrderElimination
{
    public class ResearchOrder : Order
    {
        private const float amountOfExpirience = 0.3f;
        public ResearchOrder(PlanetPoint target, Squad squad) : base(target, squad) { }
        public override void Start() { }
        public override void End()
        {
            _squad.DistributeExperience(base._target.GetPlanetInfo().expirience * amountOfExpirience);
        }
    }
}
