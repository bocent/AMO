using System.Collections;
using UnityEngine;

public class PlayBGM : MonoBehaviour {
    
    public AudioSource targetSource;
    public AudioClip clip;
    public bool loop = true;
    public bool playOnActive = true;
    public float delay = 0f;
    public bool changeVolume;
    public float volume = 1f;

    private void OnEnable()
    {
        if (playOnActive)
        {
            if (delay <= 0)
            {
                Play(clip);
            }
            else
            {
                StartCoroutine(Playing());
            }
        }
        if (changeVolume) ChangeVolume(volume);
    }

    private IEnumerator Playing()
    {
        yield return new WaitForSeconds(delay);
        Play(clip);
    }

    public void Play(AudioClip clip)
    {
        if (clip != null)
            SoundManager.instance.PlayBGM(clip, loop, targetSource);
    }

    public void Stop()
    {
        SoundManager.instance.StopBGM(targetSource);
    }
    public void ChangeVolume(float volume)
    {
        SoundManager.instance.bgmAudio.volume = volume;
    }
}
