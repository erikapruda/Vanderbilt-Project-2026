using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;

    [Header("Test UI Canvases")]
    [SerializeField] private Canvas stroopCanvas;
    [SerializeField] private Canvas nBackCanvas;
    [SerializeField] private Canvas emotionCanvas;
    [SerializeField] private Canvas arithmeticCanvas;

    private Canvas hiddenTestCanvas;

    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isPaused;
    private GameObject hiddenTestPrefab;
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
        isPaused = true;
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseMenuUI) pauseMenuUI.SetActive(false);

        RestoreHiddenTestCanvas();

        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        HideActiveTestCanvas();

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        
        if (optionsMenuUI != null)
            optionsMenuUI.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsMenuUI != null)
            optionsMenuUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    private void HideActiveTestCanvas()
    {
        hiddenTestCanvas = null;

        Canvas[] testCanvases =
        {
            stroopCanvas,
            nBackCanvas,
            emotionCanvas,
            arithmeticCanvas
        };

        foreach (Canvas canvas in testCanvases)
        {
            if (canvas != null && canvas.gameObject.activeInHierarchy)
            {
                hiddenTestCanvas = canvas;
                canvas.enabled = false;
                break;
            }
        }
    }

    private void RestoreHiddenTestCanvas()
    {
        if (hiddenTestCanvas != null)
        {
            hiddenTestCanvas.enabled = true;
            hiddenTestCanvas = null;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
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