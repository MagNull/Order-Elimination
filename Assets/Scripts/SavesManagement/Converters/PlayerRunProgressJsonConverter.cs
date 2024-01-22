using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    public class PlayerRunProgressJsonConverter : JsonConverter<PlayerRunProgress>
    {
        private readonly string _upgrades = nameof(PlayerRunProgress.StatUpgrades);
        private readonly string _currency = nameof(PlayerRunProgress.RoguelikeCurrency);
        private readonly string _characters = nameof(PlayerRunProgress.PosessedCharacters);

        public override PlayerRunProgress ReadJson(
            JsonReader reader, Type objectType, 
            PlayerRunProgress existingValue, bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var converters = serializer.Converters;

            var upgrades = jo[_upgrades].ToObject<StrategyStats>(serializer);
            var currency = jo[_currency].ToObject<int>(serializer);
            var characters = jo[_characters].ToObject<GameCharacter[]>(serializer);

            return new PlayerRunProgress
            {
                StatUpgrades = upgrades,
                PosessedCharacters = characters,
                RoguelikeCurrency = currency
            };
        }

        public override void WriteJson(
            JsonWriter writer, PlayerRunProgress value, JsonSerializer serializer)
        {
            var jo = new JObject();

            jo[_upgrades] = JToken.FromObject(value.StatUpgrades, serializer);
            jo[_currency] = value.RoguelikeCurrency;
            jo[_characters] = JToken.FromObject(value.PosessedCharacters, serializer);

            jo.WriteTo(writer);
        }
    }
}
