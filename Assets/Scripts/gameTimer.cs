using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public bool startTimerAuto = true;

    private float timeRemaining;
    private bool timerRunning = false;
    private bool timerFinished = false;

    [Header("End Game")]
    public GameObject endGamePanel;
    public Button returnToMenuButton;

    void Start()
    {
        int gameDurationMinutes = PlayerPrefs.GetInt("GameDuration", 1);
        timeRemaining = gameDurationMinutes * 60f;
        
        UpdateTimerDisplay();

        if (startTimerAuto)
        {
            StartTimer();
        }

        endGamePanel.SetActive(false);

        returnToMenuButton.onClick.AddListener(ReturnToMenu);
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

            Time.timeScale = 0f;
            endGamePanel.SetActive(true);
        
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

    void ReturnToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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