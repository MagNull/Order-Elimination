using UnityEngine;
using System;
using CharacterAbility;

public class BattleCharacterView : MonoBehaviour
{
    public event Action<BattleCharacterView> BattleCharacterViewClicked;
    
    [SerializeField]
    private BattleCharacter _character;
    private AbilityView[] _abilitiesView;

    public BattleCharacter Model => _character;
    public AbilityView[] AbilitiesView => _abilitiesView;

    public void Init(BattleCharacter character, AbilityView[] abilitiesView)
    {
        _character = character;
        BattleCharacterViewClicked += _character.OnClicked;
        _character.Damaged += OnDamaged;
        
        _abilitiesView = abilitiesView;
    }

    public void OnDamaged(int damage)
    {
        Debug.Log("Damaged");
    }

    public void OnDied()
    {
        // ���� ������
    }

    public void OnTurnStart()
    {
        throw new NotImplementedException();
    }

    private void OnMouseDown()
    {
        BattleCharacterViewClicked?.Invoke(this);
    }
}