using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ResultsManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TMP_Text titleText;
    public TMP_Text resultsText;
    public TMP_InputField seedInputField;

    [Header("Seed Copy UI")]
    public TMP_InputField seedBox;
    public Button copySeedButton;

    [Header("Export Location UI")]
    public TMP_InputField exportLocationBox;
    public Button copyExportLocationButton;
    public Button exportButton;

    private string lastExportPath = "";

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    void Start()
    {
        if (copySeedButton != null)
            copySeedButton.onClick.AddListener(CopySeed);

        if (copyExportLocationButton != null)
            copyExportLocationButton.onClick.AddListener(CopyExportLocation);

        if (exportButton != null)
            exportButton.onClick.AddListener(ExportResults);

        if (exportLocationBox != null)
        {
            exportLocationBox.text = "";
            exportLocationBox.readOnly = true;
        }
    }

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (seedInputField != null)
        {
            seedInputField.readOnly = true;
        }
    }

    public void ShowResults(string resultsSummary)
    {

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayResultsMusic();
        }

        if (titleText != null)
        {
            titleText.text = "Times Up!";
        }

        if (resultsText != null)
        {
            resultsText.text = resultsSummary;
        }

        if (seedInputField != null)
        {
            uint currentSeed = GameManager.Singleton != null ? GameManager.Singleton.Seed : 0;
            seedInputField.text = currentSeed.ToString();
        }

        StartCoroutine(FadeInPanel());
    }

    private IEnumerator FadeInPanel()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void CopySeed()
    {
        if (seedInputField != null)
        {
            GUIUtility.systemCopyBuffer = seedInputField.text;
            Debug.Log("Seed copied " + seedInputField.text);
        }
    }

    public void CopyExportLocation()
    {
        if (string.IsNullOrEmpty(lastExportPath)) return;

        GUIUtility.systemCopyBuffer = lastExportPath;
        Debug.Log("Export path copied: " + lastExportPath);
    }

    public void ExportResults()
    {
        string fileName = "test_results_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // Your existing export/write logic here
        // File.WriteAllText(path, csvContent);

        lastExportPath = path;

        if (exportLocationBox != null)
        {
            exportLocationBox.text = lastExportPath;
            exportLocationBox.ForceLabelUpdate();
        }

        Debug.Log("Results exported to: " + lastExportPath);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}