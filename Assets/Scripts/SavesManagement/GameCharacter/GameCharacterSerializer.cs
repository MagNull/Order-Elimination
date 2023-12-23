using Newtonsoft.Json;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    public static class GameCharacterSerializer
    {
        public static GameCharacter UnpackCharacterFromSaveData(
            GameCharacterSaveData characterSaveData,
            IDataMapping<Guid, IGameCharacterTemplate> templatesMapping)
        {
            var template = templatesMapping.GetData(characterSaveData.TemplateId);
            var character = GameCharactersFactory.CreateGameCharacter(
                template, characterSaveData.CharacterStats);
            character.CurrentHealth = characterSaveData.CurrentHealth;
            //character.SetInventory(characterSaveData.CharacterInventory);
            return character;
        }

        public static GameCharacterSaveData PackCharacterToSaveData(
            GameCharacter character,
            IDataMapping<Guid, IGameCharacterTemplate> templatesMapping)
        {
            var characterTemplateId = templatesMapping.GetKey(character.CharacterData);
            var stats = new GameCharacterStats(character.CharacterStats);
            var saveData = new GameCharacterSaveData(
                characterTemplateId, stats, character.CurrentHealth);
            return saveData;
        }
    }
}
