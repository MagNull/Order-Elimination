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
        private int _moveCount;

        private void Start()
        {
            if(StartMenuMediator.Instance.Outcome == BattleOutcome.Victory)
                StartMenuMediator.SetMoney(StartMenuMediator.Instance.Money + 1000);
            SetMoney(StartMenuMediator.Instance.Money);
            SetMoveCount(StartMenuMediator.Instance.CountMoveInSave);
            InputClass.onFinishMove += AddMoveCount;
        }

        public void SetMoney(int money)
        {
            _moneyText.text = money.ToString();
            _money = money;
        }

        public void SetMoveCount(int moveCount)
        {
            _moveCountText.text = $"Count move: {moveCount.ToString()}";
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