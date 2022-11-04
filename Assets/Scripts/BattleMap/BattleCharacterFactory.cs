using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterFactory : MonoBehaviour
{
    public GameObject charPrefab;

    public BattleCharacter Create(IBattleCharacterInfo info, CharacterSide side)
    {
        // Что должен делать метод?
        // Например, меняем цвет в зависимости от стороны
        // Возвращаем получившийся объект
        GameObject character = Instantiate(charPrefab);
        character.GetComponent<PlayerTestScript>().SetSide(side);
        return character.GetComponent<BattleCharacter>();
    }
}
