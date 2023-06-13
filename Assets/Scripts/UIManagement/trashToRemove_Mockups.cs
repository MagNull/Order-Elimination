using OrderElimination;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.trashToRemove_Mockups
{
    public class CharacterUpgradeTransaction
    {
        private int Money = 1000;
        public CharacterTemplate TargetCharacter { get; private set; }
        public int Cost => TargetCharacter.GetStrategyStats().CostOfUpgrade;

        public const int MaximumLevelCap = 10;

        public CharacterUpgradeTransaction(CharacterTemplate target)
        {
            TargetCharacter = target;
        }

        public bool TryUpgrade()
        {
            if (TargetCharacter.GetStrategyStats().Lvl >= MaximumLevelCap)
                return false;
            var availableMoney = Money;
            if (Cost > availableMoney)
                return false;
            TargetCharacter.Upgrade();
            availableMoney -= Cost;
            //_playerInformation.SetMoney(availableMoney);
            return true;
        }
    }

    public class BattleResult
    {
        public readonly BattleOutcome Outcome;
        public readonly CharacterTemplate[] SquadCharacters;
        public readonly int PrimaryCurrencyReceived;
        public readonly int SpecialCurrencyReceived;

        public BattleResult(BattleOutcome outcome, CharacterTemplate[] squadCharacters, int primaryCurrencyReceived, int specialCurrencyReceived)
        {
            Outcome = outcome;
            SquadCharacters = squadCharacters;
            PrimaryCurrencyReceived = primaryCurrencyReceived;
            SpecialCurrencyReceived = specialCurrencyReceived;
        }
    }

    public class MasterVolume
    {
        public static float SoundVolume { get; set; }
        public static float MusicVolume { get; set; }

        //public static void SetSoundVolume(float fractuteValue) { }
        //public static void SetMusicVolume(float fractuteValue) { }
    }
}