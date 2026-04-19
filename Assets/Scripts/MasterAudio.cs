using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

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

        if (masterSlider != null) masterSlider.value = masterVol;
        if (musicSlider != null) musicSlider.value = musicVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;

        audioMixer.SetFloat("MasterVolume", masterVol);
        audioMixer.SetFloat("MusicVolume", musicVol);
        audioMixer.SetFloat("SFXVolume", sfxVol);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }
}