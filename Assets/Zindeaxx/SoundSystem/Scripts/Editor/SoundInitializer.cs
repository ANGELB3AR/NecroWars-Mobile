using UnityEditor;
using UnityEngine;

namespace Zindeaxx.SoundSystem
{

#if UNITY_EDITOR
    [InitializeOnLoad]
    public class SoundInitializer
    {
        static SoundInitializer()
        {
            DefineSymbols.Add("ZINDEAXX_SOUNDSYSTEM");
            if (!PlayerPrefs.HasKey("ZINDEAXX_SOUNDSYSTEM_INFOSHOW"))
            {
                SoundSystemTutorialWindow.ShowWindow();
            }
        }
    }
#endif
}