using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private IntFillBar musicIntFillBar;
    [SerializeField] private IntFillBar sfxIntFillBar;
    [SerializeField] private UnityEvent<float> updateMusicVolume;
    [SerializeField] private UnityEvent<float> updateSFXVolume;

    private const string MUSIC_VOLUME_CODE = "Music Volume";
    private const string SFX_VOLUME_CODE = "SFX Volume";

    private void Awake()
    {
        musicIntFillBar.OnValueUpdated += UpdateMusicVolume;
        sfxIntFillBar.OnValueUpdated += UpdateSFXVolume;
        
        var musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_CODE, 1f);
        var SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_CODE, 1f);

        musicIntFillBar.SetFillAmount(musicVolume);
        sfxIntFillBar.SetFillAmount(SFXVolume);
    }

    private void UpdateMusicVolume(int value, float fillAmount)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_CODE, fillAmount);
        updateMusicVolume.Invoke(fillAmount);
    }

    private void UpdateSFXVolume(int value, float fillAmount)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_CODE, fillAmount);
        updateSFXVolume.Invoke(fillAmount);
    }
}
