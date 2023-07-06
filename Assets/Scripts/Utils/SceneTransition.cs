using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrderElimination
{
    public class SceneTransition
    {
        public void LoadBattleMap()
        {
            PlayerPrefs.SetInt("sceneIndex", 2);
            LoadSceneLoader();
        }

        public void LoadRoguelikeMap()
        {
            PlayerPrefs.SetInt("sceneIndex", 1);
            LoadSceneLoader();
        }
        public void LoadStartSessionMenu()
        {
            PlayerPrefs.SetInt("sceneIndex", 0);
            LoadSceneLoader();
        }

        private void LoadSceneLoader()
        {
            SceneManager.LoadSceneAsync(3);
        }
    }
}