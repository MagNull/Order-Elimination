using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class Saves : MonoBehaviour
    {
        [SerializeField] private Button _loadButton;
        private List<GameObject> _saves;
        private Image _selectedSavesImages;
        private Color _activeImageColor = Color.white;
        private Color _defaultImageColor = Color.gray;
        public static event Action<bool> ExitSavesWindow;
        public static event Action<int> LoadClicked;
        public static event Action<int> NewGameClicked;
        
        
        private void Awake()
        {
            Database.LoadSave += SetSaveText;
            _saves = new List<GameObject>
            {
                GameObject.Find("FirstSave"),
                GameObject.Find("SecondSave"),
                GameObject.Find("ThirdSave")
            };
            
            FirstSaveClicked();
            _loadButton.interactable = true;
        }

        public void FirstSaveClicked()
        {
            _selectedSavesImages = _saves[0].GetComponent<Image>();
            SwitchColorButtons();
        }
        
        public void SecondSaveClicked()
        {
            _selectedSavesImages =_saves[1].GetComponent<Image>();
            SwitchColorButtons();
        }

        public void ThirdSaveClicked()
        {
            _selectedSavesImages = _saves[2].GetComponent<Image>();
            SwitchColorButtons();
        }

        private void SwitchColorButtons()
        {
            foreach (var image in _saves.Select(save => save.GetComponent<Image>()))
            {
                image.color = _selectedSavesImages == image
                    ? _activeImageColor
                    : _defaultImageColor;
            }

            _loadButton.interactable = !IsEmptySave(GetIndexSave());
        }

        private bool IsEmptySave(int saveIndex)
        {
            return _saves[GetIndexSave()].GetComponentInChildren<TMP_Text>().text == "";
        }

        public void SetSaveText(int index, string text)
        {
            _saves[index].GetComponentInChildren<TMP_Text>().text = text;
        }

        public void LoadButtonClicked()
        {
            LoadClicked?.Invoke(GetIndexSave());
            SceneManager.LoadScene("StrategyMap");
        }

        public void NewGameButtonClicked()
        {
            NewGameClicked?.Invoke(GetIndexSave());
            SceneManager.LoadScene("StrategyMap");
        }

        private int GetIndexSave()
        {
            for (var i = 0; i < _saves.Count; i++)
            {
                if (_saves[i].GetComponent<Image>() != _selectedSavesImages)
                    continue;
                return i;
            }

            throw new Exception("Not selected save");
        }

        public void ExitClicked()
        {
            ExitSavesWindow?.Invoke(true);
            gameObject.SetActive(false);
        }
    }
}