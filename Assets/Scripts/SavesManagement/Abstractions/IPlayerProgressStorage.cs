namespace OrderElimination.SavesManagement
{
    public interface IPlayerProgressStorage
    {
        public bool ContainsProgressData(PlayerData player);

        public PlayerProgressSerializableData GetPlayerProgress(PlayerData player);

        public void SetPlayerProgress(PlayerData player, PlayerProgressSerializableData saveData);
    }
}
