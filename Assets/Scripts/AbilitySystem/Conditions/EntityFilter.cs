using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class EntityFilter : ICloneable<EntityFilter>
    {
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

        public EntityFilter Clone()
        {
            var clone = new EntityFilter();
            clone.AllowedEntityTypes = AllowedEntityTypes.Clone();
            clone.AllowedRelationships = AllowedRelationships.Clone();
            clone.AllowSelf = AllowSelf;
            return clone;
        }

        public bool IsAllowed(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entity) //TODO: change to asking player
        {
            if (askingEntity == entity)
                return AllowSelf;
            var relationship = battleContext.GetRelationship(askingEntity.BattleSide, entity.BattleSide);
            return AllowedEntityTypes[entity.EntityType] && AllowedRelationships[relationship];
        }
    }
}
