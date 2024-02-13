using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    public static SoundEffectPlayer Instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundClip[] sounds;

    // Sounds
    public const string COIN = "Coin";
    public const string POWER_UP = "PowerUp";
    public const string POWER_DOWN = "PowerDown";
    public const string KACHING = "Kaching";


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"Too many of {this.GetType()} Deleting this one");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySoundClip(string soundName)
    {
        var soundClip = sounds.FirstOrDefault(x => x.soundName == soundName);
        if (soundClip != null && soundClip.audioClips.Length != 0)
        {
            audioSource.PlayOneShot(soundClip.audioClips.Random());
        }
    }

    [Serializable]
    private class SoundClip
    {
        public string soundName;
        public AudioClip[] audioClips;
    }
}
