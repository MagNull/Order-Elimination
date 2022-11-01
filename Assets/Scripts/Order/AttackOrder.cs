using OrderElimination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination
{
    public class AttackOrder : Order
    {
        public AttackOrder(PlanetPoint target, Squad squad) : base(target, squad) { }
        
        public override void Start()
        {
            _target.MoveSquad(_squad);
        }

        public override void End()
        {
            base._squad.DistributeExperience(base._target.GetPlanetInfo().Expirience);
        }
    }
}
