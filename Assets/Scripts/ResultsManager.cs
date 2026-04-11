using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TMP_Text titleText;
    public TMP_Text resultsText;
    public TMP_InputField seedInputField;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

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

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}