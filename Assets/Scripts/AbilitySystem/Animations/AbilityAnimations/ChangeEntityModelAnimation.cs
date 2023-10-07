using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class ChangeEntityModelAnimation : AwaitableAbilityAnimation
    {
        public enum ModelChangeType
        {
            ToNew,
            ToDefault
        }

        [ShowInInspector, OdinSerialize]
        public ActionEntity EntityToChangeIcon { get; set; }

        [ShowInInspector, OdinSerialize]
        public ModelChangeType ChangeType { get; set; } = ModelChangeType.ToNew;
        //changetype: to default / to new
        //change icon / change visual model

        [EnableIf(
            "@" + nameof(ChangeType) + "==" + nameof(ModelChangeType) + "." + nameof(ModelChangeType.ToNew))]
        [ShowInInspector, OdinSerialize]
        public Sprite NewIcon { get; set; }

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            var entitiesBank = context.SceneContext.EntitiesBank;
            var entityView = EntityToChangeIcon switch
            {
                ActionEntity.Caster => context.CasterView,
                ActionEntity.Target => context.TargetView,
                _ => throw new NotImplementedException(),
            };
            var icon = ChangeType switch
            {
                ModelChangeType.ToDefault => entityView.BattleEntity.EntityType switch
                {
                    EntityType.Character => entitiesBank.GetBasedCharacter(entityView.BattleEntity).CharacterData.BattleIcon,
                    EntityType.Structure => entitiesBank.GetBasedStructureTemplate(entityView.BattleEntity).BattleIcon,
                    _ => throw new NotImplementedException(),
                },
                ModelChangeType.ToNew => NewIcon,
                _ => throw new NotImplementedException(),
            };
            entityView.BattleIcon = icon;
        }
    }
}
