using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleCharacterView : MonoBehaviour
{
    // Action<> - �� ����? ��� ������� �����?
    public static event Action BattleCharacterViewClicked;

    private BattleCharacter _character;
    private AbilityView[] _abilitiesView;

    public BattleCharacter GetModel() => _character;
    public AbilityView[] GetAbilitiesView() => _abilitiesView;

    public void Init(BattleCharacter character, AbilityView[] abilitiesView)
    {
        _character = character;
        // ��� ��������?
        _abilitiesView = abilitiesView;
    }

    public void Clicked()
    {
        BattleCharacterViewClicked?.Invoke();
    }

    public void OnDied()
    {
        // ���� ������
    }
}
