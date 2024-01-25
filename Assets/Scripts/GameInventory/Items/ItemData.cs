using System;
using System.Collections.Generic;
using AI;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameInventory.Items
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Others
    }

    public enum EquipmentType
    {
        Bonus,
        Upgrade
    }

    public enum ItemRarity
    {
        Common,
        Rare,
        Epic,
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class ItemData : SerializedScriptableObject, IGuidAsset
    {
        #region Guid
        [PropertyOrder(-1)]
        [OdinSerialize, DisplayAsString]
        public Guid AssetId { get; private set; }
        public void UpdateId(Guid id) => AssetId = id;
        #endregion

        public EnumMask<Role> RoleFilter = new();
        public List<IGameCharacterTemplate> CharacterFilter = new();
        [field: SerializeField] 
        public ItemView View { get; private set; }
        [field: SerializeField] 
        public ItemType Type { get; private set; }

        [field: SerializeField] 
        public ItemRarity Rarity { get; private set; }

        [field: ShowInInspector, DisplayAsString]
        public string Id { get; private set; }//TODO-SAVES: replace with AssetId

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public ActiveAbilityBuilder UseAbility { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Consumable")]
        public int UseTimes { get; private set; }

        [field: SerializeField, ShowIf("@Type == ItemType.Equipment")]
        public EquipmentType EquipType { get; private set; }

        [field: SerializeReference, ShowIf("@Type == ItemType.Equipment && EquipType == EquipmentType.Bonus")]
        public PassiveAbilityBuilder EquipAbility { get; private set; }

        [field: SerializeField, ShowIf("@Type == ItemType.Equipment && EquipType == EquipmentType.Upgrade")]
        public SerializedDictionary<ActiveAbilityBuilder, ActiveAbilityBuilder> AbilitySwapTable = new();
        
        [field: SerializeField]
        public bool HideInInventory;

        private void Awake()
        {
            if (AssetId == Guid.Empty)
                AssetId = Guid.NewGuid();
        }
    }
}