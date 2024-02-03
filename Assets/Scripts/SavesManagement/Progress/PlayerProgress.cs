using System;
namespace OrderElimination.SavesManagement
{
    public class PlayerProgress : IPlayerProgress
    {
        public PlayerMetaProgress MetaProgress { get; set; }
        public PlayerRunProgress CurrentRunProgress { get ; set; }
        public DateTime SaveDate { get; set; }
    }
}
