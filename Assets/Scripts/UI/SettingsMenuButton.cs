using Managers.Display;
using Managers.Sound;
using UnityEngine;

namespace UI
{
    public class SettingsMenuButton : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _audioSettings;
        [SerializeField] private GameObject _displaySettings;
        [SerializeField] private GameObject _controlsSettings;
        
        private GameObject _currentSettings;
        
        private void Start()
        {
            _currentSettings = _audioSettings;
            _currentSettings.SetActive(true);
        }
        
        public void ChangeSettings(GameObject newSettings)
        {
            _currentSettings.SetActive(false);
            _currentSettings = newSettings;
            _currentSettings.SetActive(true);
        }
        
        public void ButtonReset(GameObject settings)
        {
            if (settings == _audioSettings)
            {
                SoundManager.Instance.ResetSettings();
            }
            else if (settings == _displaySettings)
            {
                DisplayManager.Instance.ResetSettings();
            }
            else if (settings == _controlsSettings)
            {
                Debug.Log("Reset Controls");
            }
        }
        
        public void ButtonBack()
        {
            _mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
