namespace OrderElimination
{
    public abstract class Order
    {
        protected Point _target;
        protected Squad _squad;

        public Order(Point target, Squad squad)
        {
            _target = target;
            _squad = squad;
        }

        public abstract void Start();

        public abstract void End();
    }

}
