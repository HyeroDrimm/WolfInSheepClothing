using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private FillBar musicFillBar;
    [SerializeField] private FillBar sfxFillBar;
    [SerializeField] private Toggle vibrationsToggle;

    private void Awake()
    {
        musicFillBar.OnValueUpdated += UpdateMusicVolume;
        sfxFillBar.OnValueUpdated += UpdateSFXVolume;
        vibrationsToggle.onValueChanged.AddListener(UpdateVibrationsOn);
    }

    private void UpdateMusicVolume(float value, float fillAmount)
    {

    }

    private void UpdateSFXVolume(float value, float fillAmount)
    {

    }

    private void UpdateVibrationsOn(bool state)
    {

    }
}
