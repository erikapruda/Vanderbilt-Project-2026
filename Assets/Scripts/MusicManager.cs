using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Tracks")]
    public AudioClip gameplayTrack;
    public AudioClip resultsTrack;
    public AudioClip menuTrack;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayGameplayMusic()
    {
        PlayTrack(gameplayTrack);
    }

    public void PlayResultsMusic()
    {
        PlayTrack(resultsTrack);
    }

    private void PlayTrack(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        PlayTrack(menuTrack);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}