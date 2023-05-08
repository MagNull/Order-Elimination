using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartSessionMenu
{
    public class StartRaceButton : MonoBehaviour
    {
        public void StartRace()
        {
            SceneManager.LoadScene("RoguelikeMap");
        }
    }
}