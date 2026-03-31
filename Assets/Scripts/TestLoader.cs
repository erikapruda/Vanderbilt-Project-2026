using UnityEngine;

public class TestLoader : MonoBehaviour
{
    [Header("GameObject Prefabs")]
    public GameObject stroopPrefab;
    public GameObject emotionPrefab;
    public GameObject arithmeticPrefab;

    void Awake()
    {
        string selectedMode = PlayerPrefs.GetString("GameModifier", "");

        switch (selectedMode)
        {
            case "Stroop":
                if (stroopPrefab != null) stroopPrefab.SetActive(true);
                break;
            case "Emotion":
                if (emotionPrefab != null) emotionPrefab.SetActive(true);
                break;
            case "Arithmetic":
                if (arithmeticPrefab != null) arithmeticPrefab.SetActive(true);
                break;
            default:
                Debug.LogWarning("Nothing has been selected.");
            break;
        }
    }
}