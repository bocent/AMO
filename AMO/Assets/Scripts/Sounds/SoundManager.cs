using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Audio;

public class Sound
{
	public AudioSource audio;
	public AudioClip[] clip;

	public Sound(AudioSource audio, AudioClip[] clip)
	{
		this.audio = audio;
		this.clip = clip;
	}
}

public class SoundManager : MonoBehaviour {

    public const string BGM = "BGM";
    public const string SFX = "SFX";
    public const string Voice = "Voice";

    public AudioMixer audioMixer;
	private AudioClip myAudio = null;
	private AudioSource usedAudioSource;
	
	public string bgmPath = "Sounds/BGM/";
	public string sfxPath = "Sounds/SFX/";

	private bool isSfxOn = true;
	private bool isBgmOn = true;
    private bool isVoiceOn = true;
	
	private string audioName = "";

	private Dictionary<string, AudioClip> audioDict;
	public AudioSource bgmAudio;
	public AudioSource sfxAudio;
	public Sound sound;

	private Tweener tweenVolume;

	public static SoundManager instance;

	private void Awake(){
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
	}

	private void OnEnable()
	{
        audioDict = new Dictionary<string, AudioClip>();
		isBgmOn = true;
		isSfxOn = true;
        isVoiceOn = true;
		if(PlayerPrefs.HasKey(BGM))
		{
            isBgmOn = PlayerPrefs.GetInt(BGM) == 1 ? true : false;
		}
		else
		{
			isBgmOn = true;
		}
		Debug.LogError ("IS BGM ON : " + isBgmOn);
		if(PlayerPrefs.HasKey(SFX))
		{
			if(PlayerPrefs.GetInt(SFX) == 1)
			{
				isSfxOn = true;
			}
			else
			{
				isSfxOn = false;
			}
		}
		else
		{
			isSfxOn = true;
		}
	}

	public void PlayBGM(string name, float delay, bool loop = false, AudioSource audio = null)
	{
        if (audio == null)
            audio = bgmAudio;
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
                tweenVolume.Pause();

            audio.volume = 1;
            myAudio = (AudioClip)Resources.Load(bgmPath + name);
            audio.clip = myAudio;
            audio.loop = loop;
            audio.PlayDelayed(delay);
            audioName = name;
        }
	}

	public void PlayBGM(string name, bool loop = false, AudioSource audio = null)
	{
        //if (audioName == name)
        //    return;
        
        if (audio == null)
            audio = bgmAudio;
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
                tweenVolume.Pause();

            audio.Stop();
            myAudio = (AudioClip)Resources.Load(bgmPath + name);
            audio.clip = myAudio;
            audio.loop = loop;
            if (isBgmOn)
            {
                audio.Play();
            }
            this.audioName = name;
        }
	}

    public IEnumerator PlayBGMFade(string name, bool loop, float fadeTime, AudioSource audio)
    {
        if (audioName == name)
            yield break;

        if (audio == null)
            audio = bgmAudio;
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
            {
                tweenVolume.Pause();
            }

            TweenVolume(1, 0, fadeTime, true, Ease.Linear, 1, LoopType.Restart);
            yield return new WaitForSeconds(fadeTime);

            audio.Stop();
            myAudio = (AudioClip)Resources.Load(bgmPath + name);
            audio.clip = myAudio;
            audio.loop = loop;
            if (isBgmOn)
            {
                audio.Play();
            }
            this.audioName = name;
            TweenVolume(0, 1, fadeTime, true, Ease.Linear, 1, LoopType.Restart);
        }
	}

    public void PlayBGM(AudioClip clip, float delay, bool loop, AudioSource audio)
    {
        if (audio == null)
            audio = bgmAudio;
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
            {
                tweenVolume.Pause();
            }

            if (audio != null)
            {
                audio.volume = 1;
                audio.clip = clip;
                audio.loop = loop;
                audio.PlayDelayed(delay);

                this.audioName = clip.name;
            }
        }
    }

    public void PlayBGM(bool loop, AudioSource audio = null)
    {
        if (audio == null)
            audio = bgmAudio;
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
            {
                tweenVolume.Pause();
            }

            audio.volume = 1;
            if (isBgmOn)
            {
                audio.loop = loop;
                audio.Play();
                //Debug.LogError ("CLIP : " + audio.clip);
            }
        }
    }


    public void PlayBGM(AudioClip clip, bool loop)
	{
        PlayBGM(clip, loop, bgmAudio);
	}

    public void PlayBGM(AudioClip clip, bool loop, AudioSource audio, float volume)
    {
        if (audio == null)
        {
            audio = bgmAudio;
        }
        usedAudioSource = audio;
        if (audio != null)
        {
            if (audio.clip == clip)
            {
                return;
            }
            if (tweenVolume != null)
            {
                tweenVolume.Pause();
            }
            usedAudioSource.volume = volume;
            if (isBgmOn)
            {
                audio.clip = clip;
                audio.loop = loop;
                audio.Play();

                this.audioName = clip.name;
            }
        }
    }

    public void PlayBGM(AudioClip clip, bool loop, AudioSource audio)
    {
        if (audio == null)
        {
            audio = bgmAudio;
        }
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
            {
                tweenVolume.Pause();
            }
            usedAudioSource.volume = 1;
            if (isBgmOn)
            {
                audio.clip = clip;
                audio.loop = loop;
                audio.Play();

                this.audioName = clip.name;
            }
        }
    }

    public void PlayBGM(AudioSource audio, AudioClip clip, float delay, bool loop)
	{
        if (audio = null)
        {
            audio = bgmAudio;
        }
        usedAudioSource = audio;
        if (audio != null)
        {
            if (tweenVolume != null)
            {
                tweenVolume.Pause();
            }
            usedAudioSource.volume = 1;
            if (isBgmOn)
            {
                audio.clip = clip;
                audio.loop = loop;
                audio.PlayDelayed(delay);

                this.audioName = clip.name;
                Debug.Log(clip.name + ", " + audio);
            }
        }
	}

	public void PlayBGM(AudioClip[] clip, AudioSource audio)
	{
		usedAudioSource = audio;
		if (tweenVolume != null) {
			tweenVolume.Pause();
		}
		usedAudioSource.volume = 1;
		if (isBgmOn) {
			sound = new Sound (audio, clip);
			StartCoroutine (PlayingBGM ());
		}
	}

	public void PlayBGM(AudioClip[] clip)
	{
		if (isBgmOn) {
			sound = new Sound (null, clip);
			StartCoroutine (PlayingBGM ());
		}
	}

    public void TweenVolume(float from, float to, float duration, bool isIgnoreTimeScale, Ease ease, int loopCount, LoopType loop)
    {
        if (usedAudioSource == null)
            usedAudioSource = GetComponent<AudioSource>();
        if (usedAudioSource != null)
        {
            usedAudioSource.volume = from;
            tweenVolume = usedAudioSource.DOFade(to, duration);
            tweenVolume.SetUpdate(isIgnoreTimeScale).SetEase(ease).SetLoops(loopCount, loop);
        }
    }

	private IEnumerator PlayingBGM()
	{
        AudioSource audio = bgmAudio;
        if (audio != null)
        {
            if (isBgmOn)
            {
                for (int i = 0; i < sound.clip.Length; i++)
                {
                    if (sound.audio == null)
                    {
                        audio.clip = sound.clip[i];
                        audio.Play();
                    }
                    else
                    {
                        sound.audio.clip = sound.clip[i];
                        sound.audio.Play();
                    }
                    this.audioName = sound.clip[i].name;
                    yield return new WaitForSeconds(sound.clip[i].length);
                }
                StartCoroutine(PlayingBGM());
            }
        }
	}
	
	public string PlayingBGMName()
	{
		if(myAudio != null)
		{
			return audioName;
		}
		else
			return null;
	}
	
	public void PlaySFX(string name, bool cache = false)
	{
        PlaySFX(name, cache, sfxAudio);
	}

    public void PlaySFX(string name, bool cache = false, AudioSource audio = null)
    {
        Debug.Log("play SFX : " + sfxPath + name);
        if (audio == null)
            audio = sfxAudio;
        Debug.Log("audio : " + audio, audio);
        if (audio != null)
        {
            AudioClip clip = null;
            if (audioDict.ContainsKey(name))
            {
                clip = audioDict[name];
            }
            else
            {
                clip = Resources.Load<AudioClip>(sfxPath + name);
                Debug.Log("clip : " + clip);
                if (cache)
                {
                    if (!audioDict.ContainsKey(name))
                    {
                        audioDict.Add(name, clip);
                    }
                }
            }

            if (isSfxOn)
            {
                audio.pitch = 1;
                audio.PlayOneShot(clip);
            }
        }
    }

    public void PlaySFX(AudioClip clip)
	{
        PlaySFX(clip, sfxAudio);
	}

    public void PlaySFX(AudioClip clip, AudioSource audio)
    {
        if (audio == null)
            audio = sfxAudio;
        if (clip == null)
        {
            return;
        }
        if (isSfxOn)
        {
            audio.pitch = 1;
            audio.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip, bool randomPitch)
	{
        PlaySFX(clip, randomPitch, sfxAudio);
	}

    public void PlaySFX(AudioClip clip, bool randomPitch, AudioSource audio)
    {
        if (audio == null)
            audio = sfxAudio;
        if (clip == null)
        {
            Error("null audio clip");
            return;
        }
        if (isSfxOn)
        {
            float rand = Random.Range(0.9f, 1.1f);
            if (randomPitch)
            {
                audio.pitch = rand;
            }
            else
            {
                audio.pitch = 1;
            }
            audio.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip, bool randomPitch, float pitch)
	{
        PlaySFX(clip, randomPitch, pitch, sfxAudio);
	}

    public void PlaySFX(AudioClip clip, bool randomPitch, float pitch, AudioSource audio)
    {
        if (audio == null)
            audio = sfxAudio;
        if (audio != null)
        {
            if (clip == null)
            {
                Error("null audio clip");
                return;
            }
            if (isSfxOn)
            {
                float rand = Random.Range(-0.1f, 0.1f);
                audio.pitch = randomPitch ? pitch + rand : pitch;
                audio.PlayOneShot(clip);
            }
        }
    }

    public void PlayLoopSFX(AudioClip clip, bool loop, AudioSource audio)
    {
        if (audio == null)
            audio = sfxAudio;
        if (audio != null)
        {
            if (clip == null)
            {
                Error("null audio clip");
                return;
            }
            if (isSfxOn)
            {
                audio.clip = clip;
                audio.loop = loop;
                audio.Play();
                audioName = name;
            }
        }
    }

	public void BGMOnRunning(bool check)
	{
		if(check == false)
		{
			usedAudioSource.Pause();
		}
		else
		{
			if(!usedAudioSource.isPlaying)
			{
				usedAudioSource.Play();
			}
		}
		isBgmOn = check;
	}

	public void BGMOn(bool check)
	{
        isBgmOn = check;
        PlayerPrefs.SetInt(BGM, isBgmOn ? 1 : 0);
        PlayerPrefs.Save();
	}

	
	public void SFXOn(bool check)
	{
		isSfxOn = check;        
		PlayerPrefs.SetInt (SFX, isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
	}

    public void VoiceOn(bool check)
    {
        isVoiceOn = check;
        PlayerPrefs.SetInt(Voice, isVoiceOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetBgmVolume(float value)
    {
        PlayerPrefs.SetFloat(BGM + "volume", value);
        PlayerPrefs.Save();

        value = (value - 1) * 80;
        audioMixer.SetFloat("bgmVol", value);
    }

    public void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat(SFX + "volume", value);
        PlayerPrefs.Save();

        value = (value - 1) * 80;
        audioMixer.SetFloat("sfxVol", value);
    }

    public void SetVoiceVolume(float value)
    {
        PlayerPrefs.SetFloat(Voice + "volume", value);
        PlayerPrefs.Save();

        value = (value - 1) * 80;
        audioMixer.SetFloat("voiceVol", value);
    }

	public void FadeBGM(float from, float to, float duration, TweenCallback callback = null)
	{
        FadeBGM(from, to, duration, usedAudioSource);
	}

    public void FadeBGM(float from, float to, float duration, AudioSource audio, TweenCallback callback = null)
    {
        if (audio == null)
            audio = bgmAudio;
        if (audio != null)
        {
            audio.volume = from;
            Tweener tween = audio.DOFade(to, duration).SetUpdate(true);
            if (callback != null)
                tween.OnComplete(callback);
            tween.Play();
        }
    }

    public void PauseBGM()
	{
        PauseBGM(usedAudioSource);
	}

    public void PauseBGM(AudioSource audio)
    {
        if (audio == null)
            audio = bgmAudio;
        audio.Pause();
    }

    public void ResumeBGM()
    {
        if (isBgmOn)
        {
            if (usedAudioSource == null)
                bgmAudio.Play();
            else
                usedAudioSource.Play();
        }
    }

	public bool GetBGMOn()
	{
		return isBgmOn;
	}

	public bool GetSFXOn()
	{
		return isSfxOn;
	}

    public bool GetVoiceOn()
    {
        return isVoiceOn;
    }

    public float GetBgmVolume()
    {
        if (PlayerPrefs.HasKey(BGM + "volume"))
        {
            return PlayerPrefs.GetFloat(BGM + "volume");
        }
        return 1;
    }

    public float GetSfxVolume()
    {
        if (PlayerPrefs.HasKey(SFX + "volume"))
        {
            return PlayerPrefs.GetFloat(SFX + "volume");
        }
        return 1;
    }

    public float GetVoiceVolume()
    {
        if (PlayerPrefs.HasKey(Voice + "volume"))
        {
            return PlayerPrefs.GetFloat(Voice + "volume");
        }
        return 1;
    }

    public void StopBGM(AudioSource audio){
		audioName = "";
        if (audio == null)
            audio = bgmAudio;
        if (audio != null)
            audio.Stop();
	}
    public void StopSFX(AudioSource audio)
    {
        audioName = "";
        if (audio == null)
            audio = sfxAudio;
        if (audio != null)
            audio.Stop();
    }
    private void Error(string desc)
	{
		Debug.LogError (desc, this);
	}
}
