using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public bool startTimerAuto = true;

    [Header("Prefabs")]
    public GameObject stroopPrefab;
    public GameObject nBackPrefab;
    public GameObject emotionPrefab;
    public GameObject arithmeticPrefab;

    private float timeRemaining;
    private bool timerRunning = false;
    private bool timerFinished = false;


    [Header("Results")]
    public ResultsManager resultsCanvas;

    void Start()
    {
        int gameDurationMinutes = PlayerPrefs.GetInt("GameDuration", 1);
        timeRemaining = gameDurationMinutes * 60f;
        
        UpdateTimerDisplay();

        if (startTimerAuto)
        {
            StartTimer();
        }
    }

    void Update()
    {
        if (!timerRunning || timerFinished)
            return;
        
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerFinished = true;
            timerRunning = false;

            UpdateTimerDisplay();

            EndGame();
        }
        else
        {
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
        timerFinished = false;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    void HidePrefab()
    {
        GameObject[] modePrefab = { stroopPrefab, nBackPrefab, emotionPrefab, arithmeticPrefab };

        foreach (GameObject prefab in modePrefab)
        {
            if (prefab != null && prefab.activeInHierarchy)
            {
                prefab.SetActive(false);
                break;
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void EndGame()
    {
        Time.timeScale = 0f;

        HidePrefab();

        string summary = "Times Up!\n\n" + 
            "Duration: " + PlayerPrefs.GetInt("GameDuration", 0) + " min\n" +
            "Mode: " + PlayerPrefs.GetString("GameModifier", "Unknown");
        
        if (resultsCanvas != null)
        {
            resultsCanvas.ShowResults(summary);
        }
        else
        {
            Debug.LogWarning("ResultsCanvas not assigned");
        }
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public bool IsTimerFinished()
    {
        return timerFinished;
    }
}