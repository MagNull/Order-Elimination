namespace OrderElimination.SavesManagement
{
    //TODO-SAVE:
    //Player must only get data from server storage
    //Instead of sending progress directly, sends player requests
    //Server modifies remote progress, depending on requests
    public interface IPlayerProgressStorage
    {
        public IPlayerProgress GetProgress(PlayerData player);

        //Cheat unsafe
        public bool SetProgress(PlayerData player, IPlayerProgress progress);

        public void ClearProgress(PlayerData player);
    }
}
