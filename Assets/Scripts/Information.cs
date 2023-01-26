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
        
        private void Awake()
        {
            if (PlayerPrefs.GetString($"{StrategyMap.SaveIndex}:BattleOutcome") == BattleOutcome.Victory.ToString())
            {
                var money = PlayerPrefs.GetInt($"{StrategyMap.SaveIndex}:Money");
                PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:Money", money + _charactersMediator.PlanetInfo.CurrencyReward);
            }
            SetMoney(PlayerPrefs.GetInt($"{StrategyMap.SaveIndex}:Money"));
            SetMoveCount(PlayerPrefs.GetInt($"{StrategyMap.SaveIndex}:CountMove"));
            InputClass.onFinishMove += AddMoveCount;
        }

        public void SetMoney(int money)
        {
            _moneyText.text = money.ToString();
            _money = money;
            OnUpdateMoney?.Invoke();
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