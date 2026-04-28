using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const float MIN_VOLUME = 0.0001f;

    void Start()
    {
        LoadVolumeSettings();

        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicSlider != null)
              musicSlider.onValueChanged.AddListener(SetMusicVolume);
        
        if (sfxSlider != null)
              sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void LoadVolumeSettings()
    {
        float masterVol = PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_KEY, 0f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 0f);

        if (masterSlider != null) 
            masterSlider.value = masterVol;
        if (musicSlider != null) 
            musicSlider.value = musicVol;
        if (sfxSlider != null) 
            sfxSlider.value = sfxVol;

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, MIN_VOLUME, 1f);
        float db = Mathf.Log10(volume) * 20f;
        audioMixer.SetFloat("MasterVolume", db);

        PlayerPrefs.SetFloat(MASTER_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, MIN_VOLUME, 1f);
        float db = Mathf.Log10(volume) * 20f;
        audioMixer.SetFloat("MusicVolume", db);

        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp(volume, MIN_VOLUME, 1f);
        float db = Mathf.Log10(volume) * 20f;
        audioMixer.SetFloat("SFXVolume", db);

        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }
}