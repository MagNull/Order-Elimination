using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SquadPanel : MonoBehaviour
{
    [SerializeField] 
    private Image _iconPrefab;
    [Inject]
    public void Squad(Squad squad)
    {
        foreach (var character in squad.Members)
        {
            var inst = Instantiate(_iconPrefab, transform);
            inst.sprite = character.BattleIcon;
        }
    }
}
