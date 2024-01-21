using Newtonsoft.Json;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    public class GameCharacterJsonConverter : JsonConverter<GameCharacter>
    {
        public override GameCharacter ReadJson(
            JsonReader reader, Type objectType, GameCharacter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            throw new NotImplementedException();
        }

        public override void WriteJson(
            JsonWriter writer, GameCharacter value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
