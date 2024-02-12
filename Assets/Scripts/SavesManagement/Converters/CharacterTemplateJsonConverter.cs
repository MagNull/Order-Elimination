using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    public class CharacterTemplateJsonConverter : JsonConverter<IGameCharacterTemplate>
    {
        private readonly string _templateToken = "TemplateId";
        private readonly IDataMapping<Guid, IGameCharacterTemplate> _templatesMap;

        public CharacterTemplateJsonConverter(
            IDataMapping<Guid, IGameCharacterTemplate> templatesMap)
        {
            _templatesMap = templatesMap;
        }

        public override IGameCharacterTemplate ReadJson(
            JsonReader reader, Type objectType,
            IGameCharacterTemplate existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var id = jo[_templateToken].ToObject<Guid>(serializer);

            var template = _templatesMap.GetData(id);
            return template;
        }

        public override void WriteJson(
            JsonWriter writer, IGameCharacterTemplate value, JsonSerializer serializer)
        {
            if (value == null) return;

            var jo = new JObject();
            var id = _templatesMap.GetKey(value);
            jo[_templateToken] = id;

            jo.WriteTo(writer);
        }
    }
}
