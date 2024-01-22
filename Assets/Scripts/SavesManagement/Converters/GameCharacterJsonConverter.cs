using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    public class GameCharacterJsonConverter : JsonConverter<GameCharacter>
    {
        private readonly string _templateToken = "TemplateId";
        private readonly string _statsToken = nameof(GameCharacter.CharacterStats);
        private readonly string _healthToken = nameof(GameCharacter.CurrentHealth);

        private readonly IDataMapping<Guid, IGameCharacterTemplate> _templatesMap;

        public GameCharacterJsonConverter(IDataMapping<Guid, IGameCharacterTemplate> templatesMap)
        {
            _templatesMap = templatesMap;
        }

        public override GameCharacter ReadJson(
            JsonReader reader, Type objectType, 
            GameCharacter existingValue, bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var id = jo[_templateToken].ToObject<Guid>();
            var stats = jo[_statsToken].ToObject<GameCharacterStats>();
            var health = jo[_healthToken].ToObject<float>();

            var template = _templatesMap.GetData(id);
            var character = GameCharactersFactory.CreateGameCharacter(template, stats);
            character.CurrentHealth = health;

            return character;
        }

        public override void WriteJson(
            JsonWriter writer, GameCharacter value, JsonSerializer serializer)
        {
            if (value == null) return;

            var jo = new JObject();
            jo[_templateToken] = _templatesMap.GetKey(value.CharacterData);
            jo[_statsToken] = JToken.FromObject(value.CharacterStats);
            jo[_healthToken] = value.CurrentHealth;
            
            jo.WriteTo(writer);
        }
    }
}
