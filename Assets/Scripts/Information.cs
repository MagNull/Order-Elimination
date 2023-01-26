using System;
using OrderElimination.Start;
using TMPro;
using UnityEngine;

namespace OrderElimination
{
    public class Information : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _moveCountText;
        private int _money;
        public int Money => _money;
        private int _moveCount;

        public Action OnUpdateMoney;
        
        private void Awake()
        {
            if (PlayerPrefs.GetString($"{StrategyMap.SaveIndex}:BattleOutcome") == BattleOutcome.Victory.ToString())
            {
                var money = PlayerPrefs.GetInt($"{StrategyMap.SaveIndex}:Money");
                PlayerPrefs.SetInt($"{StrategyMap.SaveIndex}:Money", money + 1000);
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