using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private IntFillBar musicIntFillBar;
    [SerializeField] private IntFillBar sfxIntFillBar;
    [SerializeField] private AudioMixer audioMixer;

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

        SetVolumeOfGroup(MUSIC_VOLUME_CODE, musicVolume);
        SetVolumeOfGroup(SFX_VOLUME_CODE, SFXVolume);
    }

    private void UpdateMusicVolume(int value, float fillAmount)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_CODE, fillAmount);
        SetVolumeOfGroup(MUSIC_VOLUME_CODE, fillAmount);
    }

    private void UpdateSFXVolume(int value, float fillAmount)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_CODE, fillAmount);
        SetVolumeOfGroup(SFX_VOLUME_CODE, fillAmount);
    }

    private void SetVolumeOfGroup(string groupCode, float rawPercentage)
    {
        audioMixer.SetFloat(groupCode, Mathf.Log10(Mathf.Max(rawPercentage, 0.001f)) * 20);

    }
}
