using System;

namespace OrderElimination.SavesManagement
{
    public interface IPlayerProgress
    {
        public PlayerMetaProgress MetaProgress { get; set; }
        public PlayerRunProgress CurrentRunProgress { get; set; }

        public DateTime SaveDate { get; set; }
    }
}
