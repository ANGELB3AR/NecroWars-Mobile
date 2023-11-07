using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zindeaxx;

namespace Zindeaxx.SoundSystem
{
    public class SoundSettingsPanel : MonoBehaviour
    {
        public string[] SoundIdentifier;

        [Header("Prefab Settings")]
        public SoundControl SoundControlPrefab;

        [Header("Object Settings")]
        public Transform SliderContainer;



        private void Awake()
        {
            CreateSoundSettings();
        }

        private void CreateSoundSettings()
        {
            if (SoundControlPrefab != null)
            {
                if (SoundIdentifier != null)
                {
                    foreach (string id in SoundIdentifier)
                    {
                        SoundControl control = SoundControl.Instantiate(SoundControlPrefab, SliderContainer.transform);
                        control.SetName(id);
                    }
                }
            }
            else
            {
                Debug.LogError("You need to define a main Control Prefab from which all other Sliders will be created!");
            }
        }
    }
}