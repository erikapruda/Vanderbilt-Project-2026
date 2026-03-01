using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isPaused;

    private void Start()
    {
        Resume();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        Debug.Log("Pause() called");
        isPaused = true;
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Debug.Log("Resume() called");
        isPaused = false;
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Exit Game");
    }
}