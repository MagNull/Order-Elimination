using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace OrderElimination
{
    public class SquadCommander
    {
        public Order CreateAttackOrder(PlanetPoint target, Squad squad)
        {
            return new AttackOrder(target, squad);
        }

        public Order CreateResearchOrder(PlanetPoint target, Squad squad)
        {
            return new ResearchOrder(target, squad);
        }
    }
}