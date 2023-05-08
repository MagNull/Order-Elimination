using System;
using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu;
using TMPro;
using UnityEngine;

namespace OrderElimination
{
    public class MoneyCounter : MonoBehaviour
    {
        public void Initialize(Wallet wallet)
        {
            var textMesh = GetComponent<TextMeshProUGUI>();
            textMesh.text = wallet.Money.ToString();
            wallet.ChangeMoneyEvent.AddListener((money) => textMesh.text = money.ToString());
        }
    }
}
