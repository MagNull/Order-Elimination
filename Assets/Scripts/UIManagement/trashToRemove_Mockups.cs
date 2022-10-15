using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.trashToRemove_Mockups
{
    public class MasterVolume
    {
        public static float SoundVolume { get; set; }
        public static float MusicVolume { get; set; }

        //public static void SetSoundVolume(float fractuteValue) { }
        //public static void SetMusicVolume(float fractuteValue) { }
    }

    public class PlanetPointOrderInfo
    {
        public class OrderInfoParameter
        {
            public readonly string Name;
            public readonly string Description;
            public float Value;
            [SerializeField] private readonly Sprite _icon;

            public OrderInfoParameter(string name, float value, string description = "")
            {
                Name = name; Value = value; Description = description;
            }

            public override string ToString()
                => $"{Name}: {Value}";
        }

        public List<OrderInfoParameter> ExplorationParameters
            = new List<OrderInfoParameter>()
            {
                new OrderInfoParameter("Предметы", 20, "Шанс находки предметов"),
                new OrderInfoParameter("Опыт", 30, "Процент получаемого опыта"),
                new OrderInfoParameter("Шанс отпора", 10, "Вероятность начала боя"),
            };

        public float ExploreItemsFindChance = 20;
        public float ExploreExperiencePercent = 30;
        public float ExploreAmbushChance = 10;
        public float BattleItemsFindChance = 30;
        public float BattleExperiencePercent = 100;
        public int BattleEnemyLevel = Random.Range(1, 8);
    }
}
