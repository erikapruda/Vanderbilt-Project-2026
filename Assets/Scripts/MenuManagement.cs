using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionPanel;
    public GameObject gamePrepPanel;

    [Header("TextMeshPro")]
    public TMP_Dropdown displayModeDropdown;

    [Header("Audio")]
    public AudioMixer master;
    public AudioMixer SFX;
    public AudioMixer dialog;

    [Header("Game Mode Buttons")]
    public Button[] durationButtons;
    public Button[] modifierButtons;
    public Button startButton;
    private int selectedDuration = 0;
    private string selectedModifier = "";

    private Button currentDurationButton = null;
    private Button currentModifierButton = null;

    public void Start()
    {
        durationButtons[0].onClick.AddListener(() => SelectDuration(1, durationButtons[0]));
        durationButtons[1].onClick.AddListener(() => SelectDuration(3, durationButtons[1]));
        durationButtons[2].onClick.AddListener(() => SelectDuration(5, durationButtons[2]));

        modifierButtons[0].onClick.AddListener(() => SelectModifier("Stroop", modifierButtons[0]));
        modifierButtons[1].onClick.AddListener(() => SelectModifier("Emotion", modifierButtons[1]));
        modifierButtons[2].onClick.AddListener(() => SelectModifier("Stroop", modifierButtons[2]));
        
        startButton.onClick.AddListener(StartGame);

        UpdateStartButtonState();
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

    public void OpenPrep()
    {
        mainMenuPanel.SetActive(false);
        gamePrepPanel.SetActive(true);
    }

    public void ClosePrep()
    {
        mainMenuPanel.SetActive(true);
        gamePrepPanel.SetActive(false);
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

    void SelectDuration(int duration, Button clickedButton)
    {
        selectedDuration = duration;

        if (currentDurationButton != null && currentDurationButton != clickedButton)
        {
            SetButtonVisual(currentDurationButton, false);
        }
        
        currentDurationButton = clickedButton;
        SetButtonVisual(currentDurationButton, true);

        UpdateStartButtonState();
    }

    void SelectModifier(string modifier, Button clickedButton)
    {
        selectedModifier = modifier;

        if (currentModifierButton != null && currentModifierButton != clickedButton)
        {
            SetButtonVisual(currentModifierButton, false);
        }
        
        currentModifierButton = clickedButton;
        SetButtonVisual(currentModifierButton, true);

        UpdateStartButtonState();
    }

    void SetButtonVisual(Button button, bool isSelected)
    {
        ColorBlock colors = button.colors;

        if (isSelected)
        {
            colors.normalColor = Color.gold;
            colors.selectedColor = Color.gold;
        }
        else
        {
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
        }

        button.colors = colors;
    }

    void UpdateStartButtonState()
    {
        startButton.interactable = selectedDuration > 0 && !string.IsNullOrEmpty(selectedModifier);
    }

    void StartGame()
    {
        PlayerPrefs.SetInt("GameDuration", selectedDuration);
        PlayerPrefs.SetString("GameModifier", selectedModifier);
        PlayerPrefs.Save();

        SceneManager.LoadScene(1);
    }
}