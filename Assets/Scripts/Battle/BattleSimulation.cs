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

    private BattleObjectSide _currentTurn;
    private BattleOutcome _outcome;

    private bool _isBattleEnded = false;
    private bool _isTurnChanged = true;

    private List<BattleCharacter> _characters;

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
    }

    public void Update()
    {
        CheckBattleOutcome();
        if (_outcome != BattleOutcome.Neither)
        {
            // �� ���������� �������� ������� ������������ ��������
            if (_isBattleEnded) return;
            BattleEnded?.Invoke(_outcome);
            _selectedPlayerCharacterStatsPanel.HideInfo();
            _abilityPanel.ResetAbilityButtons();
            _isBattleEnded = true;
            Debug.LogFormat("�������� ��������� - ������� {0}", _outcome == BattleOutcome.Victory ? "�����" : "��");
        }
        else
        {
            if (_currentTurn == BattleObjectSide.Ally)
            {
                // ������� ������ ���� ������ ������������ ���� ��� ��� ������� ����
                if (_isTurnChanged)
                {
                    PlayerTurnStarted?.Invoke();
                    _isTurnChanged = false;
                    Debug.Log("������� ��� ������" % Colorize.Green);
                }
            }
            else if (_currentTurn == BattleObjectSide.Enemy)
            {
                if (_isTurnChanged)
                {
                    EnemyTurnStarted?.Invoke();
                    _isTurnChanged = false;
                    Debug.Log("������� ��� ����������" % Colorize.Red);
                }

                // �������� ��
                var enemies = _characters
                    .Select(x => x)
                    .Where(x => x.Side == BattleObjectSide.Enemy);
                foreach (var enemy in enemies)
                {
                    enemy.PlayTurn();
                }

                EndTurn();
            }
        }

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
    }

    // ���������� ����� ������, �������� ������� �� ������
    // ������ �� Update()
    public void EndTurn()
    {
        _abilityPanel.ResetAbilityButtons();
        _selectedPlayerCharacterStatsPanel.HideInfo();
        SwitchTurn();
        _isTurnChanged = true;
    }

    public void SwitchTurn() => _currentTurn =
        _currentTurn == BattleObjectSide.Ally ? BattleObjectSide.Enemy : BattleObjectSide.Ally;

    public void InitializeBattlefield()
    {
        _currentTurn = BattleObjectSide.Ally;
        _outcome = BattleOutcome.Neither;

        // ������ ���� ���
        var mapIndex = _battleMapDirector.InitializeMap();

        // ����������� ������
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
        }
        _abilityViewBinder.BindAbilityButtons(_battleMapDirector.MapView, _abilityPanel, _currentTurn);
        //TODO ����������� UI
        BattleCharacterView.Selected += _selectedPlayerCharacterStatsPanel.UpdateCharacterInfo;
        BattleCharacterView.Deselected += info => _selectedPlayerCharacterStatsPanel.HideInfo();
    }
}