using UnityEngine.SceneManagement;

namespace OrderElimination
{
    public class SceneTransition
    {
        public void LoadBattleMap()
        {
            SceneManager.LoadSceneAsync(2);
        }

        public void LoadRoguelikeMap()
        {
            SceneManager.LoadSceneAsync(1);
        }
        public void LoadStartSessionMenu()
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
}