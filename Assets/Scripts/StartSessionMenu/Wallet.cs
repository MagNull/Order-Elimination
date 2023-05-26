using System;
using UnityEngine;
using UnityEngine.Events;

namespace StartSessionMenu
{
    public class Wallet
    {
        private int _money;

        public int Money => _money;

        public WalletEvent ChangeMoneyEvent;
        public Wallet(int money)
        {
            _money = money;
            PlayerPrefs.SetInt("Money", money);
            ChangeMoneyEvent = new WalletEvent();
        }
        
        public void AddMoney(int money)
        {
            _money += money;
            PlayerPrefs.SetInt("Money", money);
            ChangeMoneyEvent?.Invoke(_money);
        }

        public void SubtractMoney(int money)
        {
            if (money > _money)
            {
                throw new ArgumentException("There is less money to spend.");   
                return;
            }
            _money -= money;
            PlayerPrefs.SetInt("Money", money);
            ChangeMoneyEvent?.Invoke(_money);
        }
    }
    public class WalletEvent : UnityEvent<int> {}
}
