using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using Unity.VisualScripting;

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

    [Header("Test Options")]
    public Button colorToggle;
    public Button debtToggle;

    [Header("Seed Input")]
    public TMP_InputField seedInput;


    public void Awake()
    {
        generator.STROOP_START_TIME = 0f;
        generator_emotion.WORD_START_TIME = 0f;
        arithmetic_generator.ARITHMETIC_START_TIME = 0f;
        generator.SHOW_COLOR_RESPONSE = false;
        generator_emotion.SHOW_COLOR_RESPONSE = false;
        arithmetic_generator.SHOW_COLOR_RESPONSE = false;
        GameManager.IsUsingDebt = false;

        durationButtons[0].onClick.AddListener(() => SelectDuration(1, durationButtons[0]));
        durationButtons[1].onClick.AddListener(() => SelectDuration(2, durationButtons[1]));
        durationButtons[2].onClick.AddListener(() => SelectDuration(3, durationButtons[2]));
        durationButtons[3].onClick.AddListener(() => SelectDuration(5, durationButtons[3]));

        modifierButtons[0].onClick.AddListener(() => SelectModifier("Stroop", modifierButtons[0]));
        modifierButtons[1].onClick.AddListener(() => SelectModifier("N-back", modifierButtons[1]));
        modifierButtons[2].onClick.AddListener(() => SelectModifier("Emotion", modifierButtons[2]));
        modifierButtons[3].onClick.AddListener(() => SelectModifier("Arithmetic", modifierButtons[3]));

        colorToggle.onClick.AddListener(() => SelectColorToggle());

        debtToggle.onClick.AddListener(() => SelectDebtToggle());
        
        startButton.onClick.AddListener(StartGame);

        if (seedInput != null)
        {
            seedInput.onEndEdit.AddListener(LoadSeedConfiguration);
        }

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

    void SelectColorToggle()
    {
        
        generator.SHOW_COLOR_RESPONSE = !generator.SHOW_COLOR_RESPONSE;
        generator_emotion.SHOW_COLOR_RESPONSE = !generator_emotion.SHOW_COLOR_RESPONSE;
        arithmetic_generator.SHOW_COLOR_RESPONSE = !arithmetic_generator.SHOW_COLOR_RESPONSE;
    }

    void SelectDebtToggle()
    {
        GameManager.IsUsingDebt = !GameManager.IsUsingDebt;
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
        bool hasModeSelection = selectedDuration > 0 && !string.IsNullOrEmpty(selectedModifier);
        bool hasSeedEntry = seedInput != null && !string.IsNullOrEmpty(seedInput.text);

        startButton.interactable = hasModeSelection || hasSeedEntry;
    }

    void LoadSeedConfiguration(string seedText)
    {
        seedText = seedText.Trim();

        if (string.IsNullOrEmpty(seedText))
        {
            UpdateStartButtonState();
            return;
        }

        int seedValue;
        if (!int.TryParse(seedText, out seedValue))
        {
            seedValue = StringToSeed(seedText);
        }

        int savedDuration = PlayerPrefs.GetInt($"Seed_{seedValue}_Duration", 0);
        string savedModifier = PlayerPrefs.GetString($"Seed_{seedValue}_Modifier", "");

        if (savedDuration > 0)
        {
            selectedDuration = savedDuration;
            RestoreDurationButton(savedDuration);
        }

        if (!string.IsNullOrEmpty(savedModifier))
        {
            selectedModifier = savedModifier;
            RestoreModifierButton(savedModifier);
        }

        UpdateStartButtonState();
    }

    void RestoreDurationButton(int duration)
    {
        if (currentDurationButton != null)
        {
            SetButtonVisual(currentDurationButton, false);
        }

        Button targetButton = null;

        switch (duration)
        {
            case 1: targetButton = durationButtons[0]; break;
            case 2: targetButton = durationButtons[1]; break;
            case 3: targetButton = durationButtons[2]; break;
            case 5: targetButton = durationButtons[3]; break;
        }

        if (targetButton != null)
        {
            currentDurationButton = targetButton;
            SetButtonVisual(currentDurationButton, true);
        }
    }

    void RestoreModifierButton(string modifier)
    {
        if (currentModifierButton != null)
        {
            SetButtonVisual(currentModifierButton, false);
        }

        Button targetButton = null;

        switch (modifier)
        {
            case "Stroop": targetButton = modifierButtons[0]; break;
            case "N-back": targetButton = modifierButtons[1]; break;
            case "Emotion": targetButton = modifierButtons[2]; break;
            case "Arithmetic": targetButton = modifierButtons[3]; break;
        }

        if (targetButton != null)
        {
            currentModifierButton = targetButton;
            SetButtonVisual(currentModifierButton, true);
        }
    }

    void StartGame()
    {
        string seedText = seedInput != null ? seedInput.text.Trim() : "";
        int finalSeed;

        if (string.IsNullOrEmpty(seedText))
        {
            finalSeed = Random.Range(1, int.MaxValue);
        }
        else if (!int.TryParse(seedText, out finalSeed))
        {
            finalSeed = StringToSeed(seedText);
        }

        PlayerPrefs.SetInt("UseSeed", 1);
        PlayerPrefs.SetInt("RequestedSeed", finalSeed);

        PlayerPrefs.SetInt("GameDuration", selectedDuration);
        PlayerPrefs.SetString("GameModifier", selectedModifier);

        PlayerPrefs.SetInt($"Seed_{finalSeed}_Duration", selectedDuration);
        PlayerPrefs.SetString($"Seed_{finalSeed}_Modifier", selectedModifier);

        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    private int StringToSeed(string text)
    {
        int hash = 23;
        foreach (char c in text)
        {
            hash = hash * 31 + c;
        }
        return hash;
    }
}