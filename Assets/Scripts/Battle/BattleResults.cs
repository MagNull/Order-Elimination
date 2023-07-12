using GameInventory.Items;
using UnityEngine.Serialization;

namespace OrderElimination.Battle
{
    public enum BattleOutcome
    {
        Win,
        Lose,
    }
    
    public struct BattleResults
    {
        public BattleOutcome BattleOutcome;
        [FormerlySerializedAs("CurrencyReward")]
        public int MoneyReward;
        public Item[] ItemsReward;
    }
}