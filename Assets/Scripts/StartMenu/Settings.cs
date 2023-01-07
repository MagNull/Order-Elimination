using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrderElimination.Start
{
    public class Settings : MonoBehaviour
    {
        public static event Action<bool> ExitSettingsWindow;
        
        public void MenuButtonClicked()
        {
            SceneManager.LoadScene("StartMenu");
        }

        public void ContinueButtonClicked()
        {
            ExitClicked();
        }

        private void ExitClicked()
        {
            ExitSettingsWindow?.Invoke(true);
            gameObject.SetActive(false);
        }
    }
}