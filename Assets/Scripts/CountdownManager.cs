using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    [Header("Text Mesh Pro")]
    public TMP_Text countdownText;

    [Header("TestLoader")]
    public TestLoader testLoader;

    [Header("Game Mode Prefabs")]
    public GameObject stroopPrefab;
    public GameObject emotionPrefab;
    public GameObject arithmeticPrefab;

    void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        Time.timeScale = 0f;

        yield return StartCoroutine(ShowText("3"));
        yield return StartCoroutine(ShowText("2"));
        yield return StartCoroutine(ShowText("1"));
        yield return StartCoroutine(ShowText("GO!"));

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;

        if ( testLoader != null)
        {
            testLoader.ActivateSelectedMode();
        }
    }

    IEnumerator ShowText(string text)
    {
        countdownText.text = text;
        countdownText.alpha = 1f;
        countdownText.rectTransform.localScale = Vector3.zero;

        float growDuration = 0.25f;
        float holdDuration = 0.50f;
        float fadeDuration = 0.25f;

        Color originalColor = countdownText.color;

        float time = 0f;

        while (time < growDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / growDuration;

            countdownText.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            countdownText.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.alpha =1f;

        yield return new WaitForSecondsRealtime(holdDuration);

        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / fadeDuration;

            countdownText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        countdownText.alpha = 0f;
    }
}