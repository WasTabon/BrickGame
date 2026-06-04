using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip tapClip;
    public AudioClip transitionClip;

    private const string SfxMuteKey = "SfxMuted";
    private const string MusicMuteKey = "MusicMuted";

    private readonly List<AudioSource> sfxPool = new List<AudioSource>();
    private AudioSource musicSource;
    private int poolIndex;
    private const int PoolSize = 5;

    private bool sfxMuted;
    private bool musicMuted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxMuted = PlayerPrefs.GetInt(SfxMuteKey, 0) == 1;
        musicMuted = PlayerPrefs.GetInt(MusicMuteKey, 0) == 1;

        BuildPool();
    }

    private void BuildPool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject go = new GameObject("SfxSource_" + i);
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }

        GameObject musicGo = new GameObject("MusicSource");
        musicGo.transform.SetParent(transform);
        musicSource = musicGo.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxMuted) return;

        if (clip == null)
        {
            Debug.LogWarning("SoundManager.PlaySfx called with null clip!");
            return;
        }

        AudioSource source = sfxPool[poolIndex];
        poolIndex = (poolIndex + 1) % sfxPool.Count;
        source.clip = clip;
        source.Play();
    }

    public void PlayTap()
    {
        PlaySfx(tapClip);
    }

    public void PlayTransition()
    {
        PlaySfx(transitionClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager.PlayMusic called with null clip!");
            return;
        }

        musicSource.clip = clip;
        musicSource.mute = musicMuted;
        musicSource.Play();
    }

    public void SetSfxMuted(bool muted)
    {
        sfxMuted = muted;
        PlayerPrefs.SetInt(SfxMuteKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMusicMuted(bool muted)
    {
        musicMuted = muted;
        musicSource.mute = muted;
        PlayerPrefs.SetInt(MusicMuteKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsSfxMuted()
    {
        return sfxMuted;
    }

    public bool IsMusicMuted()
    {
        return musicMuted;
    }
}
