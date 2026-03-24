using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionPanel;
    public TMP_Dropdown displayModeDropdown;
    public AudioMixer master;
    public AudioMixer SFX;
    public AudioMixer dialog;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        mainMenuPanel.SetActive(true);
        optionPanel.SetActive(false);
    }
    
    void onStart(int index)
    {
        SetDropdownToCurrentMode();
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
        PlayerPrefs.SetInt("DisplayMode", index);
        PlayerPrefs.Save();
    }

    public void SetDisplayMode(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }

    void SetDropdownToCurrentMode()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                displayModeDropdown.value = 0;
                break;

            case FullScreenMode.Windowed:
                displayModeDropdown.value = 1;
                break;
        
            case FullScreenMode.FullScreenWindow:
                displayModeDropdown.value = 2;
                break;
        }

        displayModeDropdown.RefreshShownValue();
    }

    public void MasterVolume(float volume)
    {
        master.SetFloat("MasterVolume", volume);
    }

    public void SoundEffects(float volume)
    {
        SFX.SetFloat("SoundEffects", volume);
    }

    public void ReadingVoice(float volume)
    {
        dialog.SetFloat("ReadingVoice", volume);
    }
}