using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    [GUIColor(0.95f, 0.35f, 0.75f)]
    public class EntityFilter : ICloneable<EntityFilter>
    {
        #region OdinVisuals
        private bool _allowsCharacters => AllowedEntityTypes[EntityType.Character] == true;
        private bool _allowsStructures => AllowedEntityTypes[EntityType.Structure] == true;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (_specifiedCharacters == null) _specifiedCharacters = new();
            if (_specifiedStructures == null) _specifiedStructures = new();
        }
        #endregion

        public enum SpicificationType
        {
            ByIgnored,
            ByAllowed
        }

        [PropertyOrder(-1)]
        [ShowInInspector, OdinSerialize]
        public bool AllowSelf { get; set; }

        [TabGroup("Allowed Entity Types")]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector, OdinSerialize]
        public EnumMask<EntityType> AllowedEntityTypes = new();

        [TabGroup("Allowed Relationships")]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector, OdinSerialize]
        public EnumMask<BattleRelationship> AllowedRelationships = new();

        [TitleGroup("Allowed Characters")]
        [ShowIf("@" + nameof(_allowsCharacters))]
        [ShowInInspector, OdinSerialize]
        public SpicificationType CharactersSpecification { get; set; } = SpicificationType.ByIgnored;

        [TitleGroup("Allowed Characters")]
        [ShowIf("@" + nameof(_allowsCharacters))]
        [ShowInInspector, OdinSerialize]
        private List<IGameCharacterTemplate> _specifiedCharacters = new();

        [TitleGroup("Allowed Structures")]
        [ShowIf("@" + nameof(_allowsStructures))]
        [ShowInInspector, OdinSerialize]
        public SpicificationType StructuresSpecification { get; set; } = SpicificationType.ByIgnored;

        [TitleGroup("Allowed Structures")]
        [ShowIf("@" + nameof(_allowsStructures))]
        [ShowInInspector, OdinSerialize]
        private List<IBattleStructureTemplate> _specifiedStructures = new();

        public EntityFilter Clone()
        {
            var clone = new EntityFilter();
            clone.AllowedEntityTypes = AllowedEntityTypes.Clone();
            clone.AllowedRelationships = AllowedRelationships.Clone();
            clone.AllowSelf = AllowSelf;
            clone.CharactersSpecification = CharactersSpecification;
            clone.StructuresSpecification = StructuresSpecification;
            clone._specifiedCharacters = _specifiedCharacters.ToList();
            clone._specifiedStructures = _specifiedStructures.ToList();
            return clone;
        }

        public bool IsAllowed(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entity) //TODO: change to asking player
        {
            if (askingEntity == entity)
                return AllowSelf;
            var relationship = battleContext.GetRelationship(askingEntity.BattleSide, entity.BattleSide);
            if (!(AllowedEntityTypes[entity.EntityType] && AllowedRelationships[relationship]))
                return false;
            if (AllowedEntityTypes[EntityType.Character]
                && entity.EntityType == EntityType.Character
                && _specifiedCharacters != null)
            {
                var characterData = battleContext.EntitiesBank.GetBattleCharacterData(entity).CharacterData;
                switch (CharactersSpecification)
                {
                    case SpicificationType.ByIgnored:
                        if (_specifiedCharacters.Contains(characterData))
                            return false;
                        break;
                    case SpicificationType.ByAllowed:
                        if (!_specifiedCharacters.Contains(characterData))
                            return false;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (AllowedEntityTypes[EntityType.Structure] 
                && entity.EntityType == EntityType.Structure
                && _specifiedStructures != null)
            {
                var structureData = battleContext.EntitiesBank.GetBattleStructureData(entity);
                switch (StructuresSpecification)
                {
                    case SpicificationType.ByIgnored:
                        if (_specifiedStructures.Contains(structureData))
                            return false;
                        break;
                    case SpicificationType.ByAllowed:
                        if (!_specifiedStructures.Contains(structureData))
                            return false;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return true;
        }
    }
}
