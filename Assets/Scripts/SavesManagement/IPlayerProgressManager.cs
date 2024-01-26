namespace OrderElimination.SavesManagement
{
    public interface IPlayerProgressManager
    {
        public IPlayerProgress GetPlayerProgress();
        public void SaveProgress();
        public void ClearProgress();
    }
}
