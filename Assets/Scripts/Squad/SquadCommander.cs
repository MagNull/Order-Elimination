namespace OrderElimination
{
    public class SquadCommander
    {
        static public Order CreateAttackOrder(PlanetPoint target, Squad squad)
        {
            return new AttackOrder(target, squad);
        }

        static public Order CreateResearchOrder(PlanetPoint target, Squad squad)
        {
            return new ResearchOrder(target, squad);
        }
    }
}