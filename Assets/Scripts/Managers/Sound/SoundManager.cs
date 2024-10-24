using UnityEngine;
using Tools.Singleton;

namespace Managers.Sound
{
    public class SoundManager : Singleton<SoundManager>
    {
        private SoundSettings _settings;
        
        public void ResetSettings()
        {
            _settings = SoundSettings.DefaultSettings;
            Debug.Log("Reset Audio");
        }
        
        private struct SoundSettings
        {
            public float MasterVolume;
            public float MusicVolume;
            public float SFXVolume;
            public float UIVolume;
            
            SoundSettings(float masterVolume, float musicVolume, float sfxVolume, float uiVolume)
            {
                MasterVolume = masterVolume;
                MusicVolume = musicVolume;
                SFXVolume = sfxVolume;
                UIVolume = uiVolume;
            }
            
            public static SoundSettings DefaultSettings => new SoundSettings(1, 1, 1, 1);
        }
    }
}
