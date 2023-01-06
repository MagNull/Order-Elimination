using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrderElimination.Start
{
    public class StartMenu : MonoBehaviour
    {
        public void StartButtonClicked()
        {
            SceneManager.LoadScene("StrategyMap");
        }

        public void SettingsButtonClicked()
        {
            Debug.Log("Settings Button is clicked");
        }

        public void ExitButtonClicked()
        {
            Application.Quit();
        }
    }
}