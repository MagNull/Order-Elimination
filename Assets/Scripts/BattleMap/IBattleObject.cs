using CharacterAbility.BuffEffects;
using UnityEngine;

//TODO(Сано): Очистить IBattleObject от кучи интерфейсов
public interface IBattleObject : IDamageable, IHealable, ITickTarget, IBuffTarget, IMovable
{
    public BattleObjectSide Side { get; }
    public GameObject View { get; set; }
}