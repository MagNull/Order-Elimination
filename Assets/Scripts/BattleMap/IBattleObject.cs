using CharacterAbility.BuffEffects;
using UnityEngine;

//TODO(����): �������� IBattleObject �� ���� �����������
public interface IBattleObject : IDamageable, IHealable, ITickTarget, IBuffTarget, IMovable
{
    public BattleObjectSide Side { get; }
    public GameObject View { get; set; }
}