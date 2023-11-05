using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Zindeaxx.SoundSystem
{

    public class SoundControl : MonoBehaviour
    {

        [Header("Object Settings")]
        [SerializeField]
        private Slider AmountSlider;
        [SerializeField]
        private TMP_Text NameText;

        private string m_name;
        public string Name => m_name;


        private void Awake()
        {
            if (AmountSlider != null)
                AmountSlider.onValueChanged.AddListener(OnChangedSlider);
        }

        private void Start()
        {
            this.AmountSlider.value = GetSaveValue();
        }


        private void OnChangedSlider(float val)
        {
            ChangeVolume(m_name, val);
        }


        private float GetSaveValue()
        {
            if (PlayerPrefs.HasKey("Sound_" + m_name.ToUpper()))
                return PlayerPrefs.GetFloat("Sound_" + m_name.ToUpper());
            else
                return 0.8f;
        }

        private void SetSaveValue()
        {
            PlayerPrefs.SetFloat("Sound_" + m_name.ToUpper(), AmountSlider.value);
        }

        private void ChangeVolume(string volumeName, float vol)
        {
            SoundManager.SetVolumeControl(volumeName, vol);
            SetSaveValue();
        }

        public void SetValue(float value)
        {
            if (AmountSlider != null)
                AmountSlider.value = Mathf.Clamp(value, 0, 1);

            SetSaveValue();
        }

        public void SetName(string name)
        {
            m_name = name;
            if (NameText != null)
            {
                NameText.text = name;
            }

        }


    }
}
