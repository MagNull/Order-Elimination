using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        public Image _progressBarImage;
        [SerializeField]
        private TMP_Text _loadingText;

        private AsyncOperation _asyncOperation;

        void Start()
        {
            var sceneIndex = PlayerPrefs.GetInt("sceneIndex");
            StartCoroutine(AsyncLoadScene(sceneIndex));
        }

        private IEnumerator AsyncLoadScene(int sceneIndex)
        {
            float loadingProgress;
            _asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!_asyncOperation.isDone)
            {
                loadingProgress = Mathf.Clamp01(_asyncOperation.progress / 0.9f);
                _progressBarImage.fillAmount = loadingProgress; 
                _loadingText.text = $"Загрузка ... {loadingProgress * 100:0}%";
                yield return true;
            }
            
            _progressBarImage.gameObject.SetActive(false);
        }
    }
}