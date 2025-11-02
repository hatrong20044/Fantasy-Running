using UnityEngine;
using System.Collections.Generic;

public enum SoundType
{
    ButtonClick,
    Win,
    Lose,
    Coin,
    Background,
    Boss,
    Jump,

}

public class SoundManager : Singleton<SoundManager>
{
    [System.Serializable]
    public class Sound
    {
        public SoundType type;
        public AudioClip clip;
    }

    [Header("Sound Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<Sound> sounds;

    private Dictionary<SoundType, AudioClip> soundDict;

    protected override void Awake()
    {
        base.Awake();
        InitializeSounds();
    }

    private void InitializeSounds()
    {
        soundDict = new Dictionary<SoundType, AudioClip>();
        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.type))
                soundDict.Add(s.type, s.clip);
        }
    }

    // --- Play SFX ---
    public void PlaySFX(SoundType type)
    {
        if (soundDict.TryGetValue(type, out var clip))
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"[SoundManager] Sound not found: {type}");
        if (PlayerPrefs.HasKey(GameSetting.SFX_VOLUME))
        {
            this.SetSFXVolume(PlayerPrefs.GetFloat(GameSetting.SFX_VOLUME));
        }
    }

    // --- Play Music ---
    public void PlayMusic(SoundType type, bool loop = true)
    {
        if (soundDict.TryGetValue(type, out var clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
            Debug.LogWarning($"[SoundManager] Music not found: {type}");
        if (PlayerPrefs.HasKey(GameSetting.MUSIC_VOLUME))
        {
            this.SetMusicVolume(PlayerPrefs.GetFloat(GameSetting.MUSIC_VOLUME));
        }
    }

    public void SetMusicVolume(float volume)
    {
        this.musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        this.sfxSource.volume = Mathf.Clamp01(volume);
    }


    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ToggleSFX(bool enable)
    {
        sfxSource.mute = !enable;
    }

    public void ToggleMusic(bool enable)
    {
        musicSource.mute = !enable;
    }
}
