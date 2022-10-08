using OrderElimination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace OrderElimination
{
    public abstract class Order
    {
        protected PlanetPoint _target;
        protected Squad _squad;

        public Order(PlanetPoint target, Squad squad)
        {
            _target = target;
            _squad = squad;
        }

        public abstract void Start();

        public abstract void End();
    }

}
