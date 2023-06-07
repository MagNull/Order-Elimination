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

        public virtual void Start() { }

        public virtual void End() { }
    }

}
