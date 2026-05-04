using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManagement : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionPanel;
    public GameObject gamePrepPanel;

    [Header("TextMeshPro")]
    public TMP_Dropdown displayModeDropdown;


    [Header("Game Mode Buttons")]
    public Button[] durationButtons;
    public Button[] modifierButtons;
    public Button startButton;
    private int selectedDuration = 0;
    private string selectedModifier = "";

    private Button currentDurationButton = null;
    private Button currentModifierButton = null;

    [Header("Prompt Count Buttons")]
    public GameObject minuteButtonGroup;
    public GameObject promptButtonGroup;
    public Button[] promptButtons;

    private int selectedPromptCount = 0;
    private Button currentPromptButton = null;
    private bool usingPromptCount = false;

    [Header("Test Options")]
    public Button colorToggle;
    public Button debtToggle;
    public Button timeTypeToggle;

    [Header("Difficulty Options")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

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

        generator.ROUND_BASED = false;
        generator_emotion.ROUND_BASED = false;
        arithmetic_generator.ROUND_BASED = false;
        n_back_generator.ROUND_BASED = false;

        GameManager.IsUsingDebt = false;

        durationButtons[0].onClick.AddListener(() => SelectDuration(1, durationButtons[0]));
        durationButtons[1].onClick.AddListener(() => SelectDuration(2, durationButtons[1]));
        durationButtons[2].onClick.AddListener(() => SelectDuration(3, durationButtons[2]));
        durationButtons[3].onClick.AddListener(() => SelectDuration(5, durationButtons[3]));

        modifierButtons[0].onClick.AddListener(() => SelectModifier("Stroop", modifierButtons[0]));
        modifierButtons[1].onClick.AddListener(() => SelectModifier("N-back", modifierButtons[1]));
        modifierButtons[2].onClick.AddListener(() => SelectModifier("Emotion", modifierButtons[2]));
        modifierButtons[3].onClick.AddListener(() => SelectModifier("Arithmetic", modifierButtons[3]));

        promptButtons[0].onClick.AddListener(() => SelectPromptCount(10, promptButtons[0]));
        promptButtons[1].onClick.AddListener(() => SelectPromptCount(15, promptButtons[1]));
        promptButtons[2].onClick.AddListener(() => SelectPromptCount(20, promptButtons[2]));
        promptButtons[3].onClick.AddListener(() => SelectPromptCount(25, promptButtons[3]));

        promptButtonGroup.SetActive(false);
        minuteButtonGroup.SetActive(true);

        easyButton.onClick.AddListener(() => SetDifficulty(1, easyButton, mediumButton, hardButton));
        mediumButton.onClick.AddListener(() => SetDifficulty(2, mediumButton, easyButton, hardButton));
        hardButton.onClick.AddListener(() => SetDifficulty(3, hardButton, easyButton, mediumButton));

        colorToggle.onClick.AddListener(() => SelectColorToggle());

        debtToggle.onClick.AddListener(() => SelectDebtToggle());

        timeTypeToggle.onClick.AddListener(() => SelectTimeTypeToggle());

        startButton.onClick.AddListener(StartGame);

        if (seedInput != null)
        {
            seedInput.onEndEdit.AddListener(LoadSeedConfiguration);
        }

        UpdateStartButtonState();
        SetDifficulty(2, mediumButton, easyButton, hardButton);
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
    
    void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMenuMusic();
        }
        SetDropdownToCurrentMode();

        if (displayModeDropdown != null)
        {
            displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
        }
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

        PlayerPrefs.SetInt("DisplayMode", index);
        PlayerPrefs.Save();
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

    void SelectPromptCount(int promptCount, Button clickedButton)
    {
    selectedPromptCount = promptCount;

    if (currentPromptButton != null && currentPromptButton != clickedButton)
    {
        SetButtonVisual(currentPromptButton, false);
    }

    currentPromptButton = clickedButton;
    SetButtonVisual(currentPromptButton, true);

    UpdateStartButtonState();
    }

    void SelectColorToggle()
    {
        generator.SHOW_COLOR_RESPONSE = !generator.SHOW_COLOR_RESPONSE;
        generator_emotion.SHOW_COLOR_RESPONSE = !generator_emotion.SHOW_COLOR_RESPONSE;
        arithmetic_generator.SHOW_COLOR_RESPONSE = !arithmetic_generator.SHOW_COLOR_RESPONSE;
        n_back_generator.SHOW_COLOR_RESPONSE = !n_back_generator.SHOW_COLOR_RESPONSE;
    }

    void SelectTimeTypeToggle()
    {
    usingPromptCount = !usingPromptCount;

    generator.ROUND_BASED = usingPromptCount;
    generator_emotion.ROUND_BASED = usingPromptCount;
    arithmetic_generator.ROUND_BASED = usingPromptCount;
    n_back_generator.ROUND_BASED = usingPromptCount;

    minuteButtonGroup.SetActive(!usingPromptCount);
    promptButtonGroup.SetActive(usingPromptCount);

    selectedDuration = 0;
    selectedPromptCount = 0;

    if (currentDurationButton != null)
        SetButtonVisual(currentDurationButton, false);

    if (currentPromptButton != null)
        SetButtonVisual(currentPromptButton, false);

    currentDurationButton = null;
    currentPromptButton = null;

    UpdateStartButtonState();
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
        bool hasTimeSelection = usingPromptCount ? selectedPromptCount > 0 : selectedDuration > 0;
        bool hasModeSelection = hasTimeSelection && !string.IsNullOrEmpty(selectedModifier);
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

        int savedUsingPrompt = PlayerPrefs.GetInt($"Seed_{seedValue}_UsingPrompt", 0);
        int savedPromptCount = PlayerPrefs.GetInt($"Seed_{seedValue}_PromptCount", 0);

        usingPromptCount = (savedUsingPrompt == 1);

        generator.ROUND_BASED = usingPromptCount;
        generator_emotion.ROUND_BASED = usingPromptCount;
        arithmetic_generator.ROUND_BASED = usingPromptCount;
        n_back_generator.ROUND_BASED = usingPromptCount;

        minuteButtonGroup.SetActive(!usingPromptCount);
        promptButtonGroup.SetActive(usingPromptCount);

        if (currentDurationButton != null)
            SetButtonVisual(currentDurationButton, false);

        if (currentPromptButton != null)
            SetButtonVisual(currentPromptButton, false);

        currentDurationButton = null;
        currentPromptButton = null;

        selectedDuration = 0;
        selectedPromptCount = 0;

        if (usingPromptCount)
        {
            selectedPromptCount = savedPromptCount;
            RestorePromptButton(savedPromptCount);
        }
        else
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

    void RestorePromptButton(int promptCount)
    {
        if (currentPromptButton != null)
        {
            SetButtonVisual(currentPromptButton, false);
        }

        Button targetButton = null;

        switch (promptCount)
        {
            case 10: targetButton = promptButtons[0]; break;
            case 15: targetButton = promptButtons[1]; break;
            case 20: targetButton = promptButtons[2]; break;
            case 25: targetButton = promptButtons[3]; break;
        }

        if (targetButton != null)
        {
            currentPromptButton = targetButton;
            SetButtonVisual(currentPromptButton, true);
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
        string seedText = seedInput.text.Trim();
        int finalSeed;

        if (string.IsNullOrEmpty(seedText))
        {
            finalSeed = Random.Range(1, int.MaxValue);
        }
        else if (!int.TryParse(seedText, out finalSeed))
        {
            finalSeed = StringToSeed(seedText);
        }

        PlayerPrefs.SetInt("UsingPromptCount", usingPromptCount ? 1 : 0);
        PlayerPrefs.SetInt("PromptCount", selectedPromptCount);

        if (usingPromptCount)
        {
            generator.ROUND_NUM = selectedPromptCount;
            generator_emotion.ROUND_NUM = selectedPromptCount;
            arithmetic_generator.ROUND_NUM = selectedPromptCount;
            n_back_generator.ROUND_NUM = selectedPromptCount;
        }

        PlayerPrefs.SetInt("UseSeed", 1);
        PlayerPrefs.SetInt("RequestedSeed", finalSeed);

        PlayerPrefs.SetInt("GameDuration", selectedDuration);
        PlayerPrefs.SetString("GameModifier", selectedModifier);

        PlayerPrefs.SetInt($"Seed_{finalSeed}_Duration", selectedDuration);
        PlayerPrefs.SetString($"Seed_{finalSeed}_Modifier", selectedModifier);

        PlayerPrefs.SetInt($"Seed_{finalSeed}_UsingPrompt", usingPromptCount ? 1 : 0);
        PlayerPrefs.SetInt($"Seed_{finalSeed}_PromptCount", selectedPromptCount);

        PlayerPrefs.Save();

        Random.InitState(PlayerPrefs.GetInt("RequestedSeed", 1));

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayGameplayMusic();
        }

        SceneManager.LoadScene(1);
    }

    public void SetDifficulty(int difficulty, Button clickedButton, Button otherButton1, Button otherButton2)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
        SetButtonVisual(clickedButton, true);
        SetButtonVisual(otherButton1, false);
        SetButtonVisual(otherButton2, false);
        PlayerPrefs.Save();
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