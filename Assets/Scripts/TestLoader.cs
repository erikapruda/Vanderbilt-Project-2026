using UnityEngine;

public class TestLoader : MonoBehaviour
{
    [Header("GameObject Prefabs")]
    public GameObject stroopPrefab;
    public GameObject emotionPrefab;
    public GameObject arithmeticPrefab;

    public void ActivateSelectedMode()
    {
        if (stroopPrefab != null) stroopPrefab.SetActive(false);
        if (emotionPrefab != null) emotionPrefab.SetActive(false);
        if (arithmeticPrefab != null) arithmeticPrefab.SetActive(false);

        string selectedMode = PlayerPrefs.GetString("GameModifier", "");

        switch (selectedMode)
        {
            case "Stroop":
                stroopPrefab.SetActive(true);
                break;
            case "Emotion":
                emotionPrefab.SetActive(true);
                break;
            case "Arithmetic":
                arithmeticPrefab.SetActive(true);
                break;
            default:
                Debug.LogWarning("Nothing has been selected.");
            break;
        }
    }
}