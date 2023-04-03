using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrderElimination
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField]
        private int _amountAvailable = 10000;
        [SerializeField]
        private MetaShop _metaShop;
        [SerializeField]
        private ChoosingCharacter _choosingCharacter;

        public void Start()
        {
            var wallet = new Wallet(_amountAvailable);
            _choosingCharacter.SetWallet(wallet);
            _metaShop.SetWallet(wallet);
        }

        public void SetActiveMetaShop()
        {
            _choosingCharacter.SaveCharacters();
            _choosingCharacter.gameObject.SetActive(false);
            
            _metaShop.gameObject.SetActive(true);
        }
        
        public void SetActiveChoosingCharacters()
        {
            _metaShop.SaveStats();
            _metaShop.gameObject.SetActive(false);
            
            _choosingCharacter.gameObject.SetActive(true);
        }

        public void StartRace()
        {
            _choosingCharacter.SaveCharacters();
            _metaShop.SaveStats();
            SceneManager.LoadScene("RoguelikeMap");
        }
    }
}