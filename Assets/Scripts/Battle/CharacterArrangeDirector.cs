using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo = OrderElimination.CharacterInfo;

public class CharacterArrangeDirector : MonoBehaviour
{
    [SerializeField]
    private BattleCharacterFactory _characterFactory;
    [SerializeReference]
    private CharacterInfo[] _alliesInfo;
    [SerializeReference]
    private CharacterInfo[] _enemiesInfo;

    private BattleMap _arrangementMap;

    public void SetArrangementMap(BattleMap map) => _arrangementMap = map;

    public List<BattleCharacter> Arrange()
    {
        var characters = new List<BattleCharacter>();

        BattleCharacter[] playerSquad = _characterFactory.CreatePlayerSquad(_alliesInfo);
        for (int i = 0; i < playerSquad.Length; i++)
        {
            characters.Add(playerSquad[i]);
            playerSquad[i].Died += OnCharacterDied;
            _arrangementMap.SetCell(0, 2 * i, playerSquad[i]);
        }

        BattleCharacter[] enemySquad = _characterFactory.CreateEnemySquad(_enemiesInfo);
        for (int i = 0; i < enemySquad.Length; i++)
        {
            characters.Add(enemySquad[i]);
            enemySquad[i].Died += OnCharacterDied;
            _arrangementMap.SetCell(_arrangementMap.Width - 1, i, enemySquad[i]);
        }

        return characters;
    }

    private void OnCharacterDied(BattleCharacter battleObject)
    {
        _arrangementMap.DestroyObject(battleObject);
        battleObject.Died -= OnCharacterDied;
    }
}
