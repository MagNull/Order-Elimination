using UnityEngine;

namespace StartSessionMenu
{
    public class StartMusicController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource.Play();
        }
    }
}