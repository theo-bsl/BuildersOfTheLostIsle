using UnityEngine;

namespace UI
{
    public class MainMenuButtons : MonoBehaviour
    {
        [SerializeField] private GameObject _settingsMenu;
        
        public void ButtonContinue()
        {
            Debug.Log("Continue");
        }
        
        public void ButtonNewGame()
        {
            Debug.Log("New Game");
        }
        
        public void ButtonSettings()
        {
            _settingsMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        
        public void ButtonExit()
        {
            Application.Quit();
        }
    }
}
