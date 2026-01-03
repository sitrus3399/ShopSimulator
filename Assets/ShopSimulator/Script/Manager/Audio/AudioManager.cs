using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using PalmVilleAudio;

public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> _BGMSounds;
    public List<Sound> _SFXSounds;
    public List<Sound> _VOSounds;
    public List<Sound> _VideoVOSounds;
    public List<Sound> _NotifSounds;

    [Header("Audio Group Mixer")]
    [SerializeField] private AudioMixerGroup masterAudioMixer;
    [SerializeField] private AudioMixerGroup bgmAudioMixer;
    [SerializeField] private AudioMixerGroup sfxAudioMixer;
    [SerializeField] private AudioMixerGroup voAudioMixer;
    [SerializeField] private AudioMixerGroup videoVOAudioMixer;
    [SerializeField] private AudioMixerGroup notifAudioMixer;

    public delegate void GetVolumeDataDelegate(float a, float b, float c, float video, float notif);

    public event GetVolumeDataDelegate OnVolumeDataChange;

    protected override void DoOnAwake()
    {
        foreach (var s in _BGMSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = bgmAudioMixer;
            s.source.clip = s.klip;
            s.source.volume = s.vol;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLoop;
        }

        foreach (var s in _SFXSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxAudioMixer;
            s.source.clip = s.klip;
            s.source.volume = s.vol;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLoop;
        }

        foreach (var s in _VOSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = voAudioMixer;
            s.source.clip = s.klip;
            s.source.volume = s.vol;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLoop;
        }

        foreach (var s in _VideoVOSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = videoVOAudioMixer;
            s.source.clip = s.klip;
            s.source.volume = s.vol;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLoop;
        }

        foreach (var s in _NotifSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = videoVOAudioMixer;
            s.source.clip = s.klip;
            s.source.volume = s.vol;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLoop;
        }
    }

    private void Start()
    {
        OnVolumeDataChange += AudioManager_OnVolumeDataChange;
    }

    protected override void DoOnDestroy()
    {
        OnVolumeDataChange -= AudioManager_OnVolumeDataChange;
    }


    private void AudioManager_OnVolumeDataChange(float bgm, float sfx, float voice, float videovo, float notif)
    {
    }

    #region Volume Controls

    public void SetVolume(SoundType tmpSoundType, float tmpVolume)
    {
        string parameter = "BGM";
        switch (tmpSoundType)
        {
            case SoundType.MASTER:
                parameter = "MASTER";
                masterAudioMixer.audioMixer.SetFloat(parameter, Mathf.Log10(tmpVolume) * 20);
                break;
            case SoundType.BGM:
                parameter = "BGM";
                bgmAudioMixer.audioMixer.SetFloat(parameter, Mathf.Log10(tmpVolume) * 20);
                break;
            case SoundType.SFX:
                parameter = "SFX";
                sfxAudioMixer.audioMixer.SetFloat(parameter, Mathf.Log10(tmpVolume) * 20);
                break;
            case SoundType.VOICE:
                parameter = "VOICE";
                voAudioMixer.audioMixer.SetFloat(parameter, Mathf.Log10(tmpVolume) * 20);
                break;
            case SoundType.VideoVO:
                parameter = "VIDEOVO";
                videoVOAudioMixer.audioMixer.SetFloat(parameter, Mathf.Log10(tmpVolume) * 20);
                break;
            case SoundType.NOTIF:
                parameter = "NOTIF";
                voAudioMixer.audioMixer.SetFloat(parameter, Mathf.Log10(tmpVolume) * 20);
                break;
            default:
                break;
        }

        if (tmpVolume <= 0)
        {
            MuteVolume(tmpSoundType, true);
        }
        else
        {
            //MuteVolume(tmpSoundType, false);
        }

        PlayerPrefs.SetFloat(parameter, tmpVolume);
    }

    public void MuteVolume(SoundType tmpSoundType, bool isMuted)
    {
        string parameter = "MASTER";
        switch (tmpSoundType)
        {
            case SoundType.MASTER:
                parameter = "MASTER";
                break;
            case SoundType.BGM:
                parameter = "BGM";
                break;
            case SoundType.SFX:
                parameter = "SFX";
                break;
            case SoundType.VOICE:
                parameter = "VOICE";
                break;
            case SoundType.VideoVO:
                parameter = "VIDEOVO";
                break;
            case SoundType.NOTIF:
                parameter = "NOTIF";
                break;
            default:
                break;
        }

        masterAudioMixer.audioMixer.SetFloat(parameter, isMuted ? -80f : 0f);
    }

    #endregion

    #region Audio Playback Functions

    public void PlayBGM(string name)
    {
        StopAllBGM();

        Sound tmpSound = _BGMSounds.Find(x => x.soundName == name);
        if (tmpSound != null)
        {
            tmpSound.source.Play();
        }
    }

    public void PlayBGMOver(string name)
    {
        Sound tmpSound = _BGMSounds.Find(x => x.soundName == name);
        if (tmpSound != null)
        {
            tmpSound.source.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound tmpSound = _SFXSounds.Find(x => x.soundName == name);
        if (tmpSound != null)
        {
            tmpSound.source.Play();
        }
    }

    public void PlaySFXSpecificSource(string name, AudioSource source)
    {
        Sound tmpSound = _SFXSounds.Find(x => x.soundName == name);
        if (tmpSound != null)
        {
            AudioSource.PlayClipAtPoint(tmpSound.source.clip, new Vector3(5, 1, 2));
            source.clip = tmpSound.source.clip;
            source.volume = tmpSound.source.volume;
            source.pitch = tmpSound.source.pitch;
            source.Play();
        }
    }
    
    public void PlaySfxAtPoint(string clipName, Vector3 position)
    {
        var tmpSound = _SFXSounds.Find(x => x.soundName == clipName);
        if (tmpSound != null)
            AudioSource.PlayClipAtPoint(tmpSound.source.clip, position, tmpSound.source.volume);
    }

    public void PlayLoopingSFX(string name)
    {
        Sound tmpSound = _SFXSounds.Find(x => x.soundName == name);
        if (tmpSound != null && !tmpSound.source.isPlaying)
        {
            tmpSound.source.Play();
        }
    }

    public void StopSFX(string name)
    {
        Sound tmpSound = _SFXSounds.Find(x => x.soundName == name);
        if (tmpSound != null)
        {
            tmpSound.source.Stop();
        }
    }

    public void PlayClick()
    {
        Sound tmpSound = _SFXSounds.Find(x => x.soundName == "Click");
        if (tmpSound != null)
        {
            tmpSound.source.Play();
        }
    }

    public void PlayVoice(string name)
    {
        Sound tmpSound = _VOSounds.Find(x => x.soundName == name);
        if (tmpSound != null)
        {
            tmpSound.source.Play();
        }
    }

    public void PlayVideoSound(string name)
    {
        StopAllVideoSound();

        Sound tmpSound = _VideoVOSounds.Find(x => x.soundName == name);
        if (tmpSound.klip)
        {
            tmpSound.source.Play();
        }
    }

    public void PlayVideoSoundOver(string name)
    {
        Sound tmpSound = _VideoVOSounds.Find(x => x.soundName == name);
        if (tmpSound.klip)
        {
            tmpSound.source.Play();
        }
    }

    public void PlayNotif(string name)
    {
        Sound tmpSound = _NotifSounds.Find(x => x.soundName == name);
        if (tmpSound.klip)
        {
            tmpSound.source.Play();
        }
    }

    public void StopAllBGM()
    {
        foreach (Sound tmpSound in _BGMSounds)
        {
            tmpSound.source.Stop();
        }
    }

    public void StopAllSFX()
    {
        foreach (Sound tmpSound in _SFXSounds)
        {
            tmpSound.source.Stop();
        }
    }

    public void StopAllVoice()
    {
        foreach (Sound tmpSound in _VOSounds)
        {
            tmpSound.source.Stop();
        }
    }

    public void StopAllVideoSound()
    {
        foreach (Sound tmpSound in _VideoVOSounds)
        {
            tmpSound.source.Stop();
        }
    }

    public void StopAllNotif()
    {
        foreach (Sound tmpSound in _NotifSounds)
        {
            tmpSound.source.Stop();
        }
    }

    #endregion
}