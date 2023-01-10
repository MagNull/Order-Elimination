using OrderElimination.Start;
using TMPro;
using UnityEngine;

namespace OrderElimination
{
    public class Information : MonoBehaviour
    {
        [SerializeField] private TMP_Text _money;
        [SerializeField] private TMP_Text _moveCount;

        private void Start()
        {
            if(StartMenuMediator.Instance.Outcome == BattleOutcome.Victory)
                StartMenuMediator.SetMoney(StartMenuMediator.Instance.Money + 1000);
            SetMoney(StartMenuMediator.Instance.Money);
            SetMoveCount(StartMenuMediator.Instance.CountMoveInSave);
        }

        public void SetMoney(int money)
        {
            _money.text = money.ToString();
        }

        public void SetMoveCount(int moveCount)
        {
            _moveCount.text = $"Count move: {moveCount.ToString()}";
        }
    }
}