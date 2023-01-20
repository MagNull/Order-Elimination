using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrderElimination.Start
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private SavesMenu _savesMenu;
        public static event Action<bool> ExitSettingsWindow;
        
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void MenuButtonClicked()
        {
            SceneManager.LoadScene(0);
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

        public void LoadButtonClicked()
        {
            SetActive(false);
            _savesMenu.SetActive(true);
        }
    }
}