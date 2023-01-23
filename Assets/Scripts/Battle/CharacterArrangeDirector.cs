using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

[Serializable]
public class CharacterArrangeDirector
{
    private readonly BattleCharacterFactory _characterFactory;
    private readonly CharactersBank _charactersBank;
    private readonly List<IBattleCharacterInfo> _enemiesInfo;

    private BattleMap _arrangementMap;
    private CharactersMediator _charactersMediator;

    [Inject]
    private CharacterArrangeDirector(CharactersMediator charactersMediator, BattleCharacterFactory characterFactory,
        CharactersBank charactersBank)
    {
        _charactersMediator = charactersMediator;
        _characterFactory = characterFactory;
        _charactersBank = charactersBank;
        _enemiesInfo = _charactersMediator.GetBattleEnemyInfo();
    }

    public void SetArrangementMap(BattleMap map) => _arrangementMap = map;

    public List<BattleCharacter> Arrange(List<Vector2Int> unitPositions,
        List<Vector2Int> enemyPositions)
    {
        var characters = new List<BattleCharacter>();

        List<BattleCharacter> playerSquad =
            _characterFactory.CreatePlayerSquad(_charactersMediator.GetBattleCharactersInfo());
        for (int i = 0; i < playerSquad.Count; i++)
        {
            characters.Add(playerSquad[i]);
            playerSquad[i].Died += OnCharacterDied;
            _arrangementMap.MoveTo(playerSquad[i], unitPositions[i].x, unitPositions[i].y);
        }

        List<BattleCharacter> enemySquad = _characterFactory.CreateEnemySquad(_enemiesInfo);
        for (int i = 0; i < enemySquad.Count; i++)
        {
            characters.Add(enemySquad[i]);
            enemySquad[i].Died += OnCharacterDied;
            _arrangementMap.MoveTo(enemySquad[i], enemyPositions[i].x, enemyPositions[i].y);
        }
        
        _charactersBank.AddCharactersRange(characters);

        return characters;
    }

    private void OnCharacterDied(BattleCharacter battleObject)
    {
        _arrangementMap.DestroyObject(battleObject);
        _charactersBank.RemoveCharacter(battleObject);
        battleObject.Died -= OnCharacterDied;
    }
}