using System;
using OrderElimination.Start;
using TMPro;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class Information : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _moveCountText;
        private int _money;
        private int _moveCount;
        private CharactersMediator _charactersMediator;
        public int Money => _money;
        public Action OnUpdateMoney;
        
        [Inject]
        private void Construct(CharactersMediator charactersMediator)
        {
            _charactersMediator = charactersMediator;
        }      
        
        private void Start()
        {
            InputClass.onFinishMove += AddMoveCount;
        }

        public void SetMoney(int money)
        {
            _moneyText.text = money.ToString();
            _money = money;
            OnUpdateMoney?.Invoke();
        }

        public void SetMoneyWithBattleOutcome()
        {
            var money = PlayerPrefs.GetInt($"{StrategyMap.SaveIndex}:Money");
            if (PlayerPrefs.GetString($"{StrategyMap.SaveIndex}:BattleOutcome") == BattleOutcome.Victory.ToString())
                money += _charactersMediator.PlanetInfo.CurrencyReward;
            _moneyText.text = money.ToString();
            _money = money;
        }

        public void SetMoveCount(int moveCount)
        {
            _moveCountText.text = $"Ход: {moveCount.ToString()}";
            _moveCount = moveCount;
        }

        public void AddMoveCount()
        {
            _moveCount++;
            SetMoveCount(_moveCount);
        }

        private void OnDisable()
        {
            InputClass.onFinishMove -= AddMoveCount;
        }
    }
}