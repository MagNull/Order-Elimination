using System;
using UnityEngine;
using UnityEngine.Events;

namespace StartSessionMenu
{
    public class Wallet
    {
        private int? _localMoney;
        private Func<int> _moneyGetter;
        private Action<int> _moneySetter;

        public Wallet(int money)
        {
            _moneyGetter = () => _localMoney.Value;
            _moneySetter = value => _localMoney = value;
            _localMoney = money;
        }

        public Wallet(Func<int> valueGetter, Action<int> valueSetter)
        {
            _moneyGetter = valueGetter;
            _moneySetter = valueSetter;
        }

        public int Money
        {
            get => _moneyGetter();
            set
            {
                if (value < 0)
                    return;
                _moneySetter(value);
                MoneyChanged?.Invoke(Money);
            }
        }

        public event Action<int> MoneyChanged; 
    }
    public class WalletEvent : UnityEvent<int> {}
}
