using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    private const string MASTER_VOLUME = "masterVolume";
    private const string MUSIC_VOLUME = "musicVolume";
    private const string VOICE_VOLUME = "voiceVolume";

    public float maxVolume = 0.2f;
    public AudioClip mainHallAudioClip;
    public AudioClip classAudioClip;

    public AudioMixer mixer;
    private AudioSource source;

    public static AudioManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
        SetMusicVolume(1f);
        SetVoiceVolume(1f);
    }

    public void LoadMusic(AudioClip clip)
    {
        source.volume = maxVolume;
        source.clip = clip;
        source.Play();
    }

    public void StopMusic(float fadeDuration)
    {
        source.DOFade(0f, fadeDuration).Play();
    }

    public void SetMasterVolume(float value)
    {
        value = 80 * value - 80;
        mixer.SetFloat(MASTER_VOLUME, value);
    }

    public float GetMasterVolume()
    {
        mixer.GetFloat(MASTER_VOLUME, out float value);
        return 1 - value / -80;
    }

    public void SetMusicVolume(float value)
    {
        value = 80 * value - 80;
        mixer.SetFloat(MUSIC_VOLUME, value);
    }

    public float GetMusicVolume()
    {
        mixer.GetFloat(MUSIC_VOLUME, out float value);
        return 1 - value / -80;
    }

    public void SetVoiceVolume(float value)
    {
        value = 20 + (100 * value - 100);
        Debug.LogError("value : " + value);
        mixer.SetFloat(VOICE_VOLUME, value);
    }

    public float GetVoiceVolume()
    {
        mixer.GetFloat(VOICE_VOLUME, out float value);
        return 1 + (value - 20) / 100;
    }

    public void PlaySFX(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}
