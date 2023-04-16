using OrderElimination;
using StartSessionMenu.ChooseCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartSessionMenu
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField]
        private MetaShop _metaShop;
        [SerializeField]
        private ChoosingCharacter _choosingCharacter;

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