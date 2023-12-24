namespace OrderElimination.SavesManagement
{
    //TODO-SAVE:
    //Player must only get data from server storage
    //Instead of sending progress directly, he sends his actions
    //Server modifies local progress, depending on these actions
    public interface IPlayerProgressStorage
    {
        public bool ContainsProgressData(PlayerData player);

        public PlayerProgressSerializableData GetPlayerProgress(PlayerData player);

        //Cheat unsafe
        public void SetPlayerProgress(PlayerData player, PlayerProgressSerializableData saveData);

        public void ClearPlayerProgress(PlayerData player);
    }
}
