namespace OrderElimination
{
    public abstract class Order
    {
        protected IPoint _target;
        protected Squad _squad;

        public Order(IPoint target, Squad squad)
        {
            _target = target;
            _squad = squad;
        }

        public abstract void Start();

        public abstract void End();
    }

}
