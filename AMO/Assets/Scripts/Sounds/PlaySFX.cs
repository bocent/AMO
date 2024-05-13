using System.Collections;
using UnityEngine;

public class PlaySFX : MonoBehaviour {
    
    public AudioSource targetSource;
    public AudioClip clip;
    public AudioClip[] clips;
    public bool loop;
    public bool playOnActive = true;
    public bool stopPreviousSfx = false;
    public AudioClip targetSfxVolumeDown;

    private void OnEnable()
    {
        if (playOnActive)
        {
            if(stopPreviousSfx)
                SoundManager.instance.StopSFX(SoundManager.instance.sfxAudio);
            Play();
        }
    }
    public void PlayIfTrue(bool value)
    {
        if (value)
        {
            Play();
        }
    }

    public void Play()
    {
        if (clip != null)
        {
            if (loop)
            {
                SoundManager.instance.PlayLoopSFX(clip, loop, targetSource);
            }
            else
            {
                SoundManager.instance.PlaySFX(clip, targetSource);
            }
            if (clip == targetSfxVolumeDown && targetSfxVolumeDown != null)
                StartCoroutine(SetBgmVolume(clip));
        }
        if (clips.Length > 0)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (loop)
                {
                    SoundManager.instance.PlayLoopSFX(clips[i], loop, targetSource);
                }
                else
                {
                    SoundManager.instance.PlaySFX(clips[i], targetSource);
                }

                if (clips[i] == targetSfxVolumeDown && targetSfxVolumeDown != null)
                    StartCoroutine(SetBgmVolume(clips[i]));
            }
        }
    }
    public void Stop()
    {
        SoundManager.instance.StopSFX(targetSource);
    }
    public void Play(AudioClip clip)
    {
        if (clip != null) SoundManager.instance.PlaySFX(clip, targetSource);
    }
    private IEnumerator SetBgmVolume(AudioClip c)
    {
        Debug.Log("audio length : " + c.length);
        yield return new WaitForSeconds(c.length);
    }
}
