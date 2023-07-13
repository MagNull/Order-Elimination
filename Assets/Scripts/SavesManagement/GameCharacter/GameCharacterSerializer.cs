using Newtonsoft.Json;
using OrderElimination.MacroGame;

namespace OrderElimination.SavesManagement
{
    public static class GameCharacterSerializer
    {
        public static GameCharacter SaveDataToCharacter(
            GameCharacterSaveData characterSaveData,
            IDataMapping<int, IGameCharacterTemplate> templatesMapping)
        {
            var template = templatesMapping.GetData(characterSaveData.BasedTemplateId);
            var character = GameCharactersFactory.CreateGameCharacter(
                template, characterSaveData.CharacterStats);
            character.CurrentHealth = characterSaveData.CurrentHealth;
            //character.SetInventory(characterSaveData.CharacterInventory);
            return character;
        }

        public static GameCharacterSaveData CharacterToSaveData(
            GameCharacter character,
            IDataMapping<int, IGameCharacterTemplate> templatesMapping)
        {
            var characterTemplateId = templatesMapping.GetKey(character.CharacterData);
            var stats = new GameCharacterStats(character.CharacterStats);
            var saveData = new GameCharacterSaveData(
                characterTemplateId, stats, character.CurrentHealth);
            return saveData;
        }
    }
}
