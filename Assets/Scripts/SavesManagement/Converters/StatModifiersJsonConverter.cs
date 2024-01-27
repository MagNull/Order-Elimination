using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public class StatModifiersJsonConverter : JsonConverter<StatModifiers>
    {
        private readonly string _modifiersToken = nameof(StatModifiers.Modifiers);

        public override StatModifiers ReadJson(
            JsonReader reader, Type objectType, 
            StatModifiers existingValue, bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var modifiers = jo[_modifiersToken]
                .ToObject<Dictionary<BattleStat, ValueModifier>>(serializer);
            return new(modifiers);
        }

        public override void WriteJson(
            JsonWriter writer, StatModifiers value, JsonSerializer serializer)
        {
            var jo = new JObject();
            jo[_modifiersToken] = JToken.FromObject(value.Modifiers, serializer);
            jo.WriteTo(writer);
        }
    }
}
