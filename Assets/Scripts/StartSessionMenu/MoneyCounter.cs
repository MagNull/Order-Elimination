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
            wallet.MoneyChanged += money => textMesh.text = money.ToString();
        }

        public void UpdateValue(float value)
        {
            var textMesh = GetComponent<TextMeshProUGUI>();
            textMesh.text = value.ToString();
        }
    }
}
