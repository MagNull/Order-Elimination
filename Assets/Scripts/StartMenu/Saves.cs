using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class Saves : MonoBehaviour
    {
        public static event Action<bool> ExitSavesWindow;
        private List<GameObject> _saves;
        private Image _selectedSavesImages;
        private Color _activeImageColor = Color.white;
        private Color _defaultImageColor = Color.gray;
        
        private void Awake()
        {
            _saves = new List<GameObject>
            {
                GameObject.Find("FirstSave"),
                GameObject.Find("SecondSave"),
                GameObject.Find("ThirdSave")
            };
            
            FirstSaveClicked();
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
        }

        private void SaveButtonClicked()
        {
            
        }
        

        public void ExitClicked()
        {
            ExitSavesWindow?.Invoke(true);
            gameObject.SetActive(false);
        }
    }
}