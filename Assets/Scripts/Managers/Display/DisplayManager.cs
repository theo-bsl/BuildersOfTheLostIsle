using Tools.Singleton;
using UnityEngine;

namespace Managers.Display
{
    public class DisplayManager : Singleton<DisplayManager>
    {
        private DisplaySettings _settings;
        
        public void ResetSettings()
        {
            _settings = DisplaySettings.DefaultSettings;
            Debug.Log("Reset Display");
        }
        
        private struct DisplaySettings
        {
            public FullScreenMode FullScreenMode;
            public float resolution;
            public bool FrameRateLimited;
            public int MaxFPS;
            
            DisplaySettings(FullScreenMode fullScreenMode, float resolution, bool frameRateLimited, int maxFps)
            {
                FullScreenMode = fullScreenMode;
                this.resolution = resolution;
                FrameRateLimited = frameRateLimited;
                MaxFPS = maxFps;
            }
            
            public static DisplaySettings DefaultSettings => new DisplaySettings(FullScreenMode.FullScreenWindow, 1, false, 60);
        }
    }
}
