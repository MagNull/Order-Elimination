using UnityEngine.SceneManagement;

namespace OrderElimination
{
    public class SceneTransition
    {
        public void LoadBattleMap()
        {
            SceneManager.LoadSceneAsync("Scenes/BattleMap");
        }

        public void LoadStrategyMap()
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}