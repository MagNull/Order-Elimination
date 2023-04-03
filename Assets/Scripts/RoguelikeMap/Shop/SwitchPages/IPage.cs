namespace OrderElimination
{
    public interface IPage
    {
        public bool IsActive { get; }
        public void ChangeVisibility(bool visibility);

    }
}