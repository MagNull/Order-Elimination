using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CharacterAbility;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.BM;
using Sirenix.OdinInspector;
using VContainer;
using UIManagement.Elements;
using UIManagement;
using UnityEngine.Rendering;

//TODO: Need full refactoring first of all
public class BattleSimulation : SerializedMonoBehaviour
{
    public static event Action PlayerTurnStarted;
    public static event Action EnemyTurnStarted;

    public static event Action<BattleOutcome> BattleEnded;

    private CharacterArrangeDirector _characterArrangeDirector;
    private BattleMapDirector _battleMapDirector;
    private AbilityViewBinder _abilityViewBinder;

    //TODO: Remove panel, use event instead call inside methods(like EndRound, BattleEnd)
    [SerializeField]
    private AbilityPanel _abilityPanel;
    [SerializeField]
    private CharacterBattleStatsPanel _selectedPlayerCharacterStatsPanel;
    [SerializeField]
    private EnemiesListPanel _enemiesListPanel;
    [SerializeField]
    private Dictionary<int, List<Vector2Int>> _unitPositions = new();
    [SerializeField]
    private Dictionary<int, List<Vector2Int>> _enemyPositions = new();

    [SerializeField]
    private static BattleObjectSide _currentTurn;

    private BattleOutcome _outcome;

    private bool _isBattleEnded = false;
    private bool _isTurnChanged = true;

    private List<BattleCharacter> _characters;
    private bool _enemyTurn = false;

    public static BattleObjectSide CurrentTurn => _currentTurn;

    [Inject]
    private void Construct(CharacterArrangeDirector characterArrangeDirector, BattleMapDirector battleMapDirector)
    {
        _battleMapDirector = battleMapDirector;
        _characterArrangeDirector = characterArrangeDirector;
    }

    private void Awake()
    {
        _characters = new List<BattleCharacter>();
        _abilityViewBinder = new AbilityViewBinder();
    }

    public void Start()
    {
        InitializeBattlefield();
        _battleMapDirector.MapView.InitStartUnitSelection();
        SimulateBattle();
    }

    public async void SimulateBattle()
    {
        CheckBattleOutcome();
        if (_outcome != BattleOutcome.Neither)
        {
            // �� ���������� �������� ������� ������������ ��������
            EndBattle();
        }
        else
        {
            if (_currentTurn == BattleObjectSide.Ally)
            {
                if (_isTurnChanged)
                {
                    PlayerTurnStarted?.Invoke();
                    _isTurnChanged = false;
                    Debug.Log("Начался ход игрока" % Colorize.Green);
                }
            }
            else if (_currentTurn == BattleObjectSide.Enemy)
            {
                if (_isTurnChanged)
                {
                    EnemyTurnStarted?.Invoke();
                    _isTurnChanged = false;
                    Debug.Log("Начался ход ИИ" % Colorize.Red);
                }

                _enemyTurn = true;
                var enemies = _characters
                    .Select(x => x)
                    .Where(x => x.Side == BattleObjectSide.Enemy);
                foreach (var enemy in enemies)
                {
                    await enemy.PlayTurn();
                }

                _enemyTurn = false;
                EndTurn();
            }
        }
    }

    private void EndBattle()
    {
        if (_isBattleEnded) return;
        BattleEnded?.Invoke(_outcome);
        _selectedPlayerCharacterStatsPanel.HideInfo();
        _abilityPanel.ResetAbilityButtons();
        _isBattleEnded = true;
        Debug.LogFormat("�������� ��������� - ������� {0}", _outcome == BattleOutcome.Victory ? "�����" : "��");
    }

    public void CheckBattleOutcome()
    {
        if (_outcome != BattleOutcome.Neither)
        {
            return;
        }

        bool isThereAnyAliveAlly = false;
        bool isThereAnyAliveEnemy = false;

        foreach (var unit in _characters)
        {
            if (unit.Stats.Health > 0)
            {
                if (unit.Side == BattleObjectSide.Ally)
                {
                    isThereAnyAliveAlly = true;
                }

                if (unit.Side == BattleObjectSide.Enemy)
                {
                    isThereAnyAliveEnemy = true;
                }
            }
        }

        _outcome = !isThereAnyAliveAlly
            ? BattleOutcome.Defeat
            : (isThereAnyAliveEnemy ? BattleOutcome.Neither : BattleOutcome.Victory);
        if(_outcome != BattleOutcome.Neither)
            EndBattle();
    }

    public void EndTurn()
    {
        if (_enemyTurn)
            return;
        _abilityPanel.ResetAbilityButtons();
        _selectedPlayerCharacterStatsPanel.HideInfo();
        SwitchTurn();
        _isTurnChanged = true;
        SimulateBattle();
    }

    public void SwitchTurn() => _currentTurn =
        _currentTurn == BattleObjectSide.Ally ? BattleObjectSide.Enemy : BattleObjectSide.Ally;

    public void InitializeBattlefield()
    {
        _currentTurn = BattleObjectSide.Ally;
        _outcome = BattleOutcome.Neither;

        var mapIndex = _battleMapDirector.InitializeMap();

        _characterArrangeDirector.SetArrangementMap(_battleMapDirector.Map);
        _characters = _characterArrangeDirector.Arrange(_unitPositions[mapIndex], _enemyPositions[mapIndex]);

        var enemies = _characters
            .Where(c => c.Side == BattleObjectSide.Enemy)
            .Select(c => c.View.GameObject.GetComponent<BattleCharacterView>())
            .ToArray();
        _enemiesListPanel.Populate(enemies);
        foreach (var e in enemies)
        {
            e.Disabled += _enemiesListPanel.RemoveItem;
            e.Disabled += view => _characters.Remove((BattleCharacter) view.Model);
        }

        foreach (var battleCharacter in _characters)
        {
            battleCharacter.Died += _ => CheckBattleOutcome();
        }

        _abilityViewBinder.BindAbilityButtons(_battleMapDirector.MapView, _abilityPanel, _currentTurn);
        //TODO ����������� UI
        BattleCharacterView.Selected += _selectedPlayerCharacterStatsPanel.UpdateCharacterInfo;
        BattleCharacterView.Deselected += info => _selectedPlayerCharacterStatsPanel.HideInfo();
    }

    private void OnDisable()
    {
        _abilityViewBinder.OnDisable(_battleMapDirector.MapView, _abilityPanel);
    }
}