using GameInventory;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public class PlayerRunProgress
    {
        // - - - Static (*resets each "new game")
        // Modifiers
        // Map (points, locations, enemies)

        // - - - Dynamic (can change during run)
        [PropertyTooltip("Валюта игрока в забеге")]
        [MinValue(0)]
        [ShowInInspector, OdinSerialize]
        public int RunCurrency { get; set; }

        //1.Replace with SquadCharacter wrapper
        //2.Characters metadata ? (id, isActiveInSquad, isHired, ...)
        //3.List ActiveCharactersIds
        [VerticalGroup("PlayerCharacters")]
        [PropertyTooltip("Персонажи игрока")]
        [ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
        [ShowInInspector, OdinSerialize]
        public List<GameCharacter> PosessedCharacters { get; set; } = new();

        [PropertyTooltip("Инвентарь игрока")]
        [ShowInInspector, OdinSerialize]
        public Inventory PlayerInventory { get; set; } = new(100);

        //TODO-SAVES: replace with Unity-object / interface. Deserialize by mapping
        [PropertyTooltip("Текущая карта")]
        [ShowInInspector, OdinSerialize]
        public Guid CurrentMapId { get; set; }

        //TODO-SAVES: replace with Unity-object / interface. Deserialize by mapping
        [PropertyTooltip("Текущая точка")]
        [ShowInInspector, OdinSerialize]
        public Guid CurrentPointId { get; set; }

        [PropertyTooltip("Пройденные точки")]
        [ShowInInspector, OdinSerialize]
        public Dictionary<Guid, bool> PassedPoints { get; set; } = new ();



        #region InspectorOnly
        #if UNITY_EDITOR
        [VerticalGroup("PlayerCharacters")]
        [Button(parameterBtnStyle: ButtonStyle.Box)]
        private void AddCharacter(CharacterTemplate template)
        {
            if (template == null)
                return;
            if (PosessedCharacters == null)
                PosessedCharacters = new();
            PosessedCharacters.Add(GameCharactersFactory.CreateGameCharacter(template));
        }

        [VerticalGroup("PlayerCharacters")]
        [EnableIf("@" + nameof(PosessedCharacters) + " != null && " 
            + nameof(PosessedCharacters) + ".Count > 0")]
        [Button]
        private void ClearCharacters()
        {
            if (PosessedCharacters == null)
                PosessedCharacters = new();
            PosessedCharacters.Clear();
        }
        #endif
        #endregion
    }
}
