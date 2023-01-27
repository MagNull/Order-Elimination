using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

public class CharacterStatsMediator: MonoBehaviour
{
    private List<CharacterStats> _characterStats;

    public IReadOnlyList<CharacterStats> CharacterStats => _characterStats;

    public static CharacterStatsMediator Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void SetCharacterStats(List<CharacterStats> characterStats)
    {
        Instance._characterStats = characterStats;
    }
}