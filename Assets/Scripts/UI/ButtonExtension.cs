using TMPro;
using UnityEngine.UI;

namespace OrderElimination.UI
{
    public static class ButtonExtension
    {
        public static void DOInterectable(this Button button, bool isInteractable)
        {
            var text = button.GetComponentInChildren<TMP_Text>();
            text.alpha = isInteractable ? 1f : 0.6f;
            button.interactable = isInteractable;
        }
    }
}